using System;
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
                    Console.Write("{0, 50}", nodeStr);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" :: {0}", node.Type?.Print(0));
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        public static void SolveFunctions(Scope scope)
        {
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                IAzTypeExpn type;

                if (fn.Type == null)
                {
                    //var tvTable = new TvTable();
                    var provider = new TvProvider();
                    var constraints = fn.Constrain(provider, scope);
                    var substitution = IConstraint.Unify(scope, constraints, provider);

                    //var tv = tvTable.GetTypeOf(fn);
                    var tv = fn.Type;
                    type = IAzTypeExpn.Substitute(tv, substitution);
                    type = TvUtils.WithUniqueTvs(type, new());
                    if (fn.ExplicitType == null)
                    {
                        (fn as AzFuncDefn).ExplicitType = type;
                    }
                }
                else
                {
                    type = fn.Type;
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\n\n{0} :: {1}", fn.Name, type.PrintWithRefinements());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(fn.Print(0));

                scope.FuncDefnTypes[fn] = type;
            }

            foreach (var inst in scope.ClassInstances)
            {
                foreach (var (_, fn) in inst.Functions)
                {
                    //var tvTable = new TvTable();
                    var provider = new TvProvider();
                    var constraints = fn.Constrain(provider, scope);
                    var substitution = IConstraint.Unify(scope, constraints, provider);

                    //var tv = tvTable.GetTypeOf(fn);
                    var tv = fn.Type;
                    var type = IAzTypeExpn.Substitute(tv, substitution);
                    type = TvUtils.WithUniqueTvs(type, new());

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("\n\n{0} :: {1}", fn.Name, type.PrintWithRefinements());
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(fn.Print(0));

                    scope.FuncDefnTypes[fn] = type;
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
