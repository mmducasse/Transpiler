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
                IAzTypeDefn.Analyze(fileScope, fileScope, azType, psType);
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
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                var tvTable = new TvTable();
                var constraints = AzFuncDefn.Constrain(tvTable, scope, fn as AzFuncDefn);
                tvTable.Print();
                var substitution = Unify(scope, constraints, tvTable.TvProvider);

                Console.WriteLine("Before Solve:");
                tvTable.Print();

                Console.WriteLine("After Solve:");
                tvTable.Print(substitution);

                var tv = tvTable.GetTypeOf(fn);
                var type = IAzTypeExpn.Substitute(tv, substitution);

                scope.FuncDefnTypes[fn] = type;
            }
        }

        private static Substitution Unify(IScope scope,
                                          ConstraintSet set,
                                          TvProvider tvProvider)
        {
            if (set.IsEmpty)
            {
                return new Substitution();
            }

            var (c, cs) = set.Next;

            // Equal types.
            if (IAzTypeExpn.Equate(c.A, c.B))
            {
                return Unify(scope, cs, tvProvider);
            }

            // A and B are both Type Variables.
            if (c.A is TypeVariable tva &&
                c.B is TypeVariable tvb)
            {
                var tvc = TvUtils.Unify(scope, tva, tvb, tvProvider);
                var sa = new Substitution(tva, tvc);
                var sb = new Substitution(tvb, tvc);
                var s = new Substitution(sa, sb);
                var s2 = Unify(scope, cs.Substitute(s), tvProvider);

                return new Substitution(s2, s);
            }

            // A is a Type Variable.
            if (c.A is TypeVariable tvaa && !Contains(c.B, tvaa))
            {
                bool doUnify = true;
                if (tvaa.HasRefinements &&
                    c.B is AzTypeSymbolExpn namedType)
                {
                    foreach (var r in tvaa.Refinements)
                    {
                        if (!scope.IsSubtypeOf(namedType.Definition, r))
                        {
                            doUnify = false;
                        }
                    }
                }

                if (doUnify)
                {
                    var s = new Substitution(tvaa, c.B);
                    var s2 = Unify(scope, cs.Substitute(s), tvProvider);

                    return new Substitution(s2, s);
                }
            }

            // B is a Type Variable.
            if (c.B is TypeVariable tvbb && !Contains(c.A, tvbb))
            {

                bool doUnify = true;
                if (tvbb.HasRefinements &&
                    c.A is AzTypeSymbolExpn namedType)
                {
                    foreach (var r in tvbb.Refinements)
                    {
                        if (!scope.IsSubtypeOf(namedType.Definition, r))
                        {
                            doUnify = false;
                        }
                    }
                }

                if (doUnify)
                {
                    var s = new Substitution(tvbb, c.A);
                    var s2 = Unify(scope, cs.Substitute(s), tvProvider);

                    return new Substitution(s2, s);
                }
            }

            // They are Function Types.
            if (c.A is AzTypeLambdaExpn fa &&
                c.B is AzTypeLambdaExpn fb)
            {
                var c1 = new Constraint(fa.Input, fb.Input, null);
                var c2 = new Constraint(fa.Output, fb.Output, null);

                var cs2 = IConstraints.Union(c1, c2, cs);

                return Unify(scope, cs2, tvProvider);
            }

            // Add case for Tuple
            // Ad case for Type ctor.

            throw Analyzer.Error("Type inference failed.", c.TEMP_Node.Position);
        }

        private static bool Contains(IAzTypeExpn t, TypeVariable tv)
        {
            return t switch
            {
                TypeVariable tv2 => tv.Equals(tv2),
                AzTypeLambdaExpn lam => Contains(lam.Input, tv) || Contains(lam.Output, tv),
                // Todo: Add tuple case.
                _ => false,
            };
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
