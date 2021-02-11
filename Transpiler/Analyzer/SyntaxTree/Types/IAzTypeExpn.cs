using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzTypeExpn : IAzNode
    {
        bool IsSolved { get; }

        public static IAzTypeExpn Analyze(Scope scope,
                                          IPsTypeExpn node)
        {
            return node switch
            {
                PsTypeSymbolExpn symExpn => AzTypeSymbolExpn.Analyze(scope, symExpn),
                PsTypeArbExpn arbExpn => AzTypeCtorExpn.Analyze(scope, arbExpn),
                PsTypeTupleExpn tupExpn => AzTypeTupleExpn.Analyze(scope, tupExpn),
                PsTypeLambdaExpn lamExpn => AzTypeLambdaExpn.Analyze(scope, lamExpn),
                _ => throw new NotImplementedException(),
            };
        }

        public static Substitution Unify(IScope scope, ConstraintSet set)
        {
            if (set.IsEmpty)
            {
                return new Substitution();
            }

            var (c, cs) = set.Next;

            // Equal types.
            if (c.A == c.B)
            {
                return Unify(scope, cs);
            }

            // A and B are both Type Variables.
            if (c.A is TypeVariable tva &&
                c.B is TypeVariable tvb)
            {
                var tvc = TvUtils.Unify(scope, tva, tvb);
                var sa = new Substitution(tva, tvc);
                var sb = new Substitution(tvb, tvc);
                var s = new Substitution(sa, sb);
                var s2 = Unify(scope, cs.Substitute(s));

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
                    var s2 = Unify(scope, cs.Substitute(s));

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
                    var s2 = Unify(scope, cs.Substitute(s));

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

                return Unify(scope, cs2);
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

        public static IAzTypeExpn Substitute(IAzTypeExpn type, Substitution sub)
        {
            if (type.IsSolved) { return type; }

            if (type is TypeVariable tv &&
                sub.TypeSubstitutions.ContainsKey(tv))
            {
                return Substitute(sub.TypeSubstitutions[tv], sub);
            }

            return type switch
            {
                AzTypeLambdaExpn lamType => AzTypeLambdaExpn.Substitute(lamType, sub),
                _ => type,
            };
        }
    }
}
