﻿using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class Analyzer
    {
        public static void Analyze(Module module, Stack<Module> dependents)
        {
            // Create the top-level scope.
            var fileScope = new Scope(Core.Instance.Scope.ToArr());
            module.Scope = fileScope;
            TvProvider tvs = new();

            // Find and analyze usings.
            AnalyzeDependencies(module, dependents);

            // Analyze types and functions in module.
            AnalyzeFile(fileScope, tvs, module.ParseResult);

            // Apply HM type inference alg to functions.
            SolveFunctions(fileScope, tvs);

            // Perform any more verification needed after type inference is done.
            PostAnalyze(fileScope);

            if (Compiler.DebugAnalyzer)
            {
                Print(module);
            }
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

                if (!module.IsAnalyzed)
                {
                    Analyze(dependency, dependents);
                }

                module.Dependencies.Add(dependency);
                module.Scope.Dependencies.Add(dependency.Scope);
            }

            dependents.Pop();
        }

        private static void AnalyzeFile(Scope fileScope, TvProvider tvs, ParseResult results)
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
                IAzTypeDefn.Analyze(fileScope, tvs, azType, psType);
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
                IAzFuncStmtDefn.Analyze(fileScope, new("p"), tvs, azFunc, psFunc);
            }

            foreach (var (psInst, azInst) in instDefnsDict)
            {
                AzClassInstDefn.Analyze(fileScope, tvs, azInst, psInst);
            }

            // Analyze all class functions in file.
            foreach (var (psClass, azClass) in classDefnsDict)
            {
                IAzTypeDefn.Analyze(fileScope, tvs, azClass, psClass);
            }

            // Analyze all instance functions in file.
            foreach (var (psClass, azClass) in classDefnsDict)
            {
                IAzTypeDefn.Analyze(fileScope, tvs, azClass, psClass);
            }
        }

        public static IReadOnlyList<IAzFuncStmtDefn> AnalyzeFunctions(Scope scope,
                                                                      TvProvider tvs,
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
                IAzFuncStmtDefn.Analyze(scope, new("p"), tvs, azFunc, psFunc);
                newFns.Add(azFunc);
            }

            return newFns;
        }

        public static void TEMP_PrintFnTypes(Scope scope)
        {
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                Console.WriteLine("\n\n {0}\n", fn.Name);
                foreach (var node in fn.GetSubnodes())
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
                }
            }
        }

        public static void SolveFunctions(Scope scope, TvProvider tvs)
        {
            // Determine a set of type constraints for the 
            // function ASTs of the file.
            ConstraintSet constraints = new();
            foreach (var funcDefn in scope.AllFunctions())
            {
                if (funcDefn.IsAutoGenerated) { continue; }
                var cs = funcDefn.Constrain(tvs, scope);
                constraints.UnionWith(cs);

                //Console.ForegroundColor = ConsoleColor.Yellow;
                //Console.WriteLine("\n\n{0} :: {1}", funcDefn.Name, funcDefn.Type.PrintWithRefinements());
                //Console.ForegroundColor = ConsoleColor.White;
                //Console.WriteLine(funcDefn.Print(0));
            }

            Substitution substitution = IConstraint.Unify(scope, constraints, tvs);

            foreach (var funcDefn in scope.AllFunctions())
            {
                funcDefn.SubstituteType(substitution);
                var uniqueSub = TvUtils.UniqueTvSubstitution(funcDefn.Type, new());
                funcDefn.SubstituteType(uniqueSub);
                scope.FuncDefnTypes[funcDefn] = funcDefn.Type;
            }

            //TEMP_PrintFnTypes(scope);
        }

        private static void PostAnalyze(Scope scope)
        {
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                if (fn is AzFuncDefn funcDefn)
                {
                    foreach (var node in funcDefn.GetSubnodes())
                    {
                        if (node is AzMatchExpn matchExpn)
                        {
                            matchExpn.PostAnalyze();
                        }
                    }
                }
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
