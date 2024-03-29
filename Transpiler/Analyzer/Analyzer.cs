﻿using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class Analyzer
    {
        public static void Analyze(Module module, Stack<Module> dependents)
        {
            if (module.IsAnalyzed) { return; }

            TypeVariables.Reset();

            // Find and analyze usings.
            AnalyzeDependencies(module, dependents);

            // Analyze types and functions in module.
            AnalyzeFile(module.Scope, module.ParseResult);

            // Apply HM type inference alg to functions.
            SolveFunctions(module.Scope);

            // Perform any more verification needed after type inference is done.
            PostAnalyze(module.Scope);

            if (Compiler.DebugAnalyzer)
            {
                Print(module);
            }

            module.IsAnalyzed = true;
        }

        private static void AnalyzeDependencies(Module module, Stack<Module> dependents)
        {
            dependents.Push(module);

            // Make sure all dependencies are analyzed first.
            foreach (var import in module.ParseResult.ImportedModules)
            {
                if (!Compiler.Instance.Modules.TryGetValue(import.ModuleName, out var dependency))
                {
                    throw Error("Unable to find module " + import.ModuleName, import.Position);
                }

                // Make sure we aren't in a cyclic dependency between modules.
                if (dependents.Contains(dependency))
                {
                    throw Error("Cyclic module dependency detected between modules " +
                                module.Name + " and " + dependency.Name + ".", import.Position);
                }

                Analyze(dependency, dependents);

                module.Dependencies.Add(dependency);
                module.Scope.Dependencies.Add(dependency.Scope);
            }

            dependents.Pop();
        }

        private static void AnalyzeFile(Scope fileScope, ParseResult results)
        {
            // Add all top-level data and union definitions to Scope.
            Dictionary<IPsTypeDefn, IAzTypeDefn> typeDefnsDict = new();
            foreach (var td in results.TypeDefns)
            {
                typeDefnsDict[td] = IAzTypeDefn.Initialize(fileScope, td);
            }

            // Add all class definitions to Scope.
            Dictionary<PsClassTypeDefn, AzClassTypeDefn> classDefnsDict = new();
            foreach (var td in results.ClassDefns)
            {
                classDefnsDict[td] = AzClassTypeDefn.Initialize(fileScope, td);
            }

            // Add all class instances to scope.
            Dictionary<PsClassInstDefn, AzClassInstDefn> instDefnsDict = new();
            foreach (var inst in results.InstDefns)
            {
                instDefnsDict[inst] = AzClassInstDefn.Initialize(fileScope, inst);
            }

            // Analyze expressions of types.
            foreach (var (psType, azType) in typeDefnsDict)
            {
                IAzTypeDefn.Analyze(fileScope, azType, psType);
            }

            // Add all top-level function definitions to scope.
            Dictionary<IAzFuncStmtDefn, IPsFuncStmtDefn> funcDefnsDict = new();
            foreach (var psFunc in results.FuncDefns)
            {
                foreach (var azFunc in IAzFuncStmtDefn.Initialize(fileScope, psFunc))
                {
                    funcDefnsDict[azFunc] = psFunc;
                }
            }

            // Analyze functions.
            foreach (var (azFunc, psFunc) in funcDefnsDict)
            {
                IAzFuncStmtDefn.Analyze(fileScope, new("p"), azFunc, psFunc);
            }

            foreach (var (psInst, azInst) in instDefnsDict)
            {
                AzClassInstDefn.Analyze(fileScope, azInst, psInst);
            }

            // Analyze all class functions in file.
            foreach (var (psClass, azClass) in classDefnsDict)
            {
                IAzTypeDefn.Analyze(fileScope, azClass, psClass);
            }

            // Analyze all instance functions in file.
            foreach (var (psClass, azClass) in classDefnsDict)
            {
                IAzTypeDefn.Analyze(fileScope, azClass, psClass);
            }
        }

        public static IReadOnlyList<IAzFuncStmtDefn> AnalyzeFunctions(Scope scope,
                                                                      IReadOnlyList<IPsFuncStmtDefn> psFuncDefns)
        {
            // Add all function definitions at this level to scope.
            Dictionary<IAzFuncStmtDefn, IPsFuncStmtDefn> funcDefnsDict = new();
            foreach (var psFunc in psFuncDefns)
            {
                foreach (var azFunc in IAzFuncStmtDefn.Initialize(scope, psFunc))
                {
                    funcDefnsDict[azFunc] = psFunc;
                }
            }

            // Analyze functions.
            List<IAzFuncStmtDefn> newFns = new();
            foreach (var (azFunc, psFunc) in funcDefnsDict)
            {
                IAzFuncStmtDefn.Analyze(scope, new("p"), azFunc, psFunc);
                newFns.Add(azFunc);
            }

            return newFns;
        }

        public static void TEMP_PrintFnTypes(Scope scope)
        {
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                Console.WriteLine("\n\n {0}\n", fn.Name);
                fn.Recurse((node) =>
                {
                    string nodeStr = node.Print(0).Replace("\n", "").Replace("\t", "");
                    if (nodeStr.Length > 45)
                    {
                        nodeStr = nodeStr[..44] + "...";
                    }
                    Console.Write("{0, 50}", nodeStr);
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    if (node.Type != null)
                    {
                        Console.WriteLine(" :: {0}", node.Type.PrintWithRefinements());
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                });
            }
        }

        public static void SolveFunctions(Scope fileScope)
        {
            var functionBatches = SolveBatcher.BatchFunctions(fileScope);

            // Determine a set of type constraints for the 
            // function ASTs of the file.
            foreach (var batch in functionBatches)
            {
                ConstraintSet constraints = new();
                foreach (var funcDefn in batch.Functions)
                {
                    if (funcDefn.IsAutoGenerated) { continue; }
                    var cs = funcDefn.Constrain();
                    constraints.UnionWith(cs);

                    //Console.ForegroundColor = ConsoleColor.Yellow;
                    //Console.WriteLine("\n\n{0} :: {1}", funcDefn.Name, funcDefn.Type.PrintWithRefinements());
                    //Console.ForegroundColor = ConsoleColor.White;
                    //Console.WriteLine(funcDefn.Print(0));
                }

                Substitution substitution = IConstraint.Unify(fileScope, constraints, TypeVariables.Provider);

                foreach (var funcDefn in batch.Functions)
                {
                    funcDefn.Recurse(n => n.SubstituteType(substitution));
                    var uniqueSub = TvUtils.UniqueTvSubstitution(funcDefn.Type, new());
                    funcDefn.Recurse(n => n.SubstituteType(uniqueSub));
                    fileScope.FuncDefnTypes[funcDefn] = funcDefn.Type;
                }
            }
        }

        private static void PostAnalyze(Scope scope)
        {
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                fn.Recurse(node =>
                {
                    if (node is AzMatchExpn matchExpn)
                    {
                        matchExpn.ValidateCases();
                    }
                });
            }
        }

        public static void Print(Module module)
        {
            var scope = module.Scope;

            foreach (var type in scope.TypeDefinitions.Values)
            {
                Console.WriteLine(type.Print(0));
            }

            foreach (var fn in scope.FuncDefinitions.Values)
            {
                Console.WriteLine("\n" + fn.Print(0) + "\n");
            }

            scope.PrintTypeHeirarchy();
        }

        public static Exception Error(string reason,
                                      CodePosition position)
        {
            return new CompilerException(eCompilerStage.Analyzer,
                                         reason,
                                         position);
        }
    }
}
