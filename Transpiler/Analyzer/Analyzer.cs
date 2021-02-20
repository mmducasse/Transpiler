﻿using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class Analyzer
    {
        public static void Analyze(Module module)
        {
            // Create the top-level scope.
            var fileScope = new Scope(CoreTypes.Instance.Scope.ToArr());
            module.Scope = fileScope;

            // Todo: Find and analyze usings.

            // Analyze types and functions in module.
            AnalyzeFile(fileScope, module.ParseResult);

            // Apply HM type inference alg to functions.
            SolveFunctions(fileScope);

            // Perform any more verification needed after type inference is done.
            PostAnalyze(fileScope);
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
            Dictionary<AzFuncDefn, PsFuncDefn> funcDefnsDict = new();
            foreach (var psFunc in results.FuncDefns)
            {
                foreach (var azFunc in AzFuncDefn.Initialize(fileScope, psFunc))
                {
                    funcDefnsDict[azFunc] = psFunc;
                }
            }

            // Analyze functions.
            //List<AzFuncDefn> newFns = new();
            foreach (var (azFunc, psFunc) in funcDefnsDict)
            {
                AzFuncDefn.Analyze(fileScope, azFunc, psFunc);
                //newFns.Add(azFunc);
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

        public static IReadOnlyList<AzFuncDefn> AnalyzeFunctions(Scope scope,
                                                                 IReadOnlyList<PsFuncDefn> psFuncDefns)
        {
            // Add all function definitions at this level to scope.
            Dictionary<AzFuncDefn, PsFuncDefn> funcDefnsDict = new();
            foreach (var psFunc in psFuncDefns)
            {
                foreach (var azFunc in AzFuncDefn.Initialize(scope, psFunc))
                {
                    funcDefnsDict[azFunc] = psFunc;
                }
            }

            // Analyze functions.
            List<AzFuncDefn> newFns = new();
            foreach (var (azFunc, psFunc) in funcDefnsDict)
            {
                AzFuncDefn.Analyze(scope, azFunc, psFunc);
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
                        Console.WriteLine(" :: {0}", node.Type.Print(0));
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }

        public static void SolveFunctions(Scope scope)
        {
            var provider = new TvProvider();

            // Determine a set of type constraints for the 
            // function ASTs of the file.
            var constraints = ConstraintSet.Empty;
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                if (fn is AzFuncDefn funcDefn)
                {
                    constraints = IConstraintSet.Union(constraints, funcDefn.Constrain(provider, scope));
                }
            }

            // Unify the constraints to generate a substitution
            // that solves for all type variables in the file.
            Substitution fileSubstitution = new Substitution();
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                if (fn is AzFuncDefn funcDefn)
                {
                    fileSubstitution = new Substitution(fileSubstitution, IConstraint.Unify(scope, constraints, provider));
                }
            }

            // Apply the substitutions to the type of each AST node.
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                if (fn is AzFuncDefn funcDefn)
                {
                    foreach (var node in fn.GetSubnodes())
                    {
                        if (node.Type != null)
                        {
                            node.Type = node.Type.Substitute(fileSubstitution);
                        }
                    }

                    funcDefn.ExplicitType = TvUtils.WithUniqueTvs(funcDefn.Type, new());

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n\n{0} :: {1}", fn.Name, funcDefn.ExplicitType.PrintWithRefinements());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(fn.Print(0));

                    scope.FuncDefnTypes[fn] = funcDefn.Type;
                }
            }

            //TEMP_PrintFnTypes(scope);

            //foreach (var inst in scope.ClassInstances)
            //{
            //    foreach (var (_, fn) in inst.Functions)
            //    {
            //        //var tvTable = new TvTable();
            //        var constraints = fn.Constrain(provider, scope);
            //        var substitution = IConstraint.Unify(scope, constraints, provider);

            //        //var tv = tvTable.GetTypeOf(fn);
            //        var tv = fn.Type;
            //        var type = IAzTypeExpn.Substitute(tv, substitution);
            //        type = TvUtils.WithUniqueTvs(type, new());

            //        Console.ForegroundColor = ConsoleColor.Yellow;
            //        Console.WriteLine("\n\n{0} :: {1}", fn.Name, type.PrintWithRefinements());
            //        Console.ForegroundColor = ConsoleColor.White;
            //        Console.WriteLine(fn.Print(0));

            //        scope.FuncDefnTypes[fn] = type;
            //    }
            //}
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
                //Console.WriteLine(fn.Print(0));
            }

            scope.PrintTypeHeirarchy();
        }

        public static Exception Error(string reason,
                                      CodePosition position)
        {
            return new InterpreterException(eInterpreterStage.Analyzer,
                                            reason,
                                            position);
        }
    }
}
