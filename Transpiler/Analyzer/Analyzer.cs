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

            // Find and analyze usings.

            // Analyze types in module.
            AnalyzeFileTypes(fileScope, module.ParseResult);

            // Analyze functions in module.
            AnalyzeFunctions(fileScope, module.ParseResult.FuncDefns);

            // Apply HM type inference alg to functions.
            SolveFunctions(fileScope);
        }

        private static void AnalyzeFileTypes(Scope fileScope, ParseResult results)
        {
            // Add all top-level data, union, class definitions to Scope.
            Dictionary<IPsTypeDefn, IAzTypeDefn> typeDefnsDict = new();
            foreach (var td in results.TypeDefns)
            {
                typeDefnsDict[td] = IAzTypeDefn.Initialize(fileScope, td);
            }

            // Add all class instances to scope

            // Analyze expressions of types.
            foreach (var (psType, azType) in typeDefnsDict)
            {
                IAzTypeDefn.Analyze(fileScope, azType, psType);
            }
        }

        public static IReadOnlyList<AzFuncDefn> AnalyzeFunctions(Scope scope, IReadOnlyList<PsFuncDefn> psFuncDefns)
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

        public static void SolveFunctions(Scope scope)
        {
            //foreach (var fn in scope.FuncDefinitions.Values)
            //{
            //    var tvTable = new TvTable();
            //    var constraints = AzFuncDefn.Constrain(tvTable, scope, fn as AzFuncDefn);
            //    var substitution = IType.Unify(scope, constraints);

            //    Console.WriteLine("Before Solve:");
            //    tvTable.Print();

            //    Console.WriteLine("After Solve:");
            //    tvTable.Print(substitution);

            //    var tv = tvTable.GetTypeOf(fn);
            //    var type = IType.Substitute(tv, substitution);

            //    scope.FuncDefnTypes[fn.Name] = type;
            //}
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
                Console.WriteLine(fn.Print(0));
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
