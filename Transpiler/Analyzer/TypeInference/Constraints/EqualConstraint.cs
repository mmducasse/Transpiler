using System;
using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    /// <summary>
    /// Constraint indicating that two types must be the same.
    /// </summary>
    public record EqualConstraint(IAzTypeExpn A,
                                  IAzTypeExpn B,
                                  IAzNode TEMP_Node) : IConstraint
    {
        public IConstraint Substitute(Substitution sub)
        {
            return new EqualConstraint(IAzTypeExpn.Substitute(A, sub),
                                       IAzTypeExpn.Substitute(B, sub),
                                       TEMP_Node);
        }

        public static Substitution Unify(IScope scope,
                                         EqualConstraint c,
                                         ConstraintSet cs,
                                         TvProvider tvProvider)
        {
            // Equal types.
            if (IAzTypeExpn.Equate(c.A, c.B))
            {
                return IConstraint.Unify(scope, cs, tvProvider);
            }

            // A and B are both Type Variables.
            if (c.A is TypeVariable tva &&
                c.B is TypeVariable tvb)
            {
                var tvc = TvUtils.Unify(scope, tva, tvb, tvProvider);
                var sa = new Substitution(tva, tvc);
                var sb = new Substitution(tvb, tvc);
                var s = new Substitution(sa, sb);
                var s2 = IConstraint.Unify(scope, cs.Substitute(s), tvProvider);

                return new Substitution(s2, s);
            }

            // A is a Type Variable.
            if (c.A is TypeVariable tvaa && !IConstraint.Contains(c.B, tvaa))
            {
                bool doUnify = true;
                if (tvaa.HasRefinements &&
                    c.B is AzTypeCtorExpn typeCtor)
                {
                    foreach (var r in tvaa.Refinements)
                    {
                        if (!scope.IsSubtypeOf(typeCtor.TypeDefn, r))
                        {
                            doUnify = false;
                        }
                    }
                }

                if (doUnify)
                {
                    var s = new Substitution(tvaa, c.B);
                    var s2 = IConstraint.Unify(scope, cs.Substitute(s), tvProvider);

                    return new Substitution(s2, s);
                }
            }

            // B is a Type Variable.
            if (c.B is TypeVariable tvbb && !IConstraint.Contains(c.A, tvbb))
            {

                bool doUnify = true;
                if (tvbb.HasRefinements &&
                    c.A is AzTypeCtorExpn typeCtor)
                {
                    foreach (var r in tvbb.Refinements)
                    {
                        if (!scope.IsSubtypeOf(typeCtor.TypeDefn, r))
                        {
                            doUnify = false;
                        }
                    }
                }

                if (doUnify)
                {
                    var s = new Substitution(tvbb, c.A);
                    var s2 = IConstraint.Unify(scope, cs.Substitute(s), tvProvider);

                    return new Substitution(s2, s);
                }
            }

            // They are Function Types.
            if (c.A is AzTypeLambdaExpn fa &&
                c.B is AzTypeLambdaExpn fb)
            {
                var c1 = new EqualConstraint(fa.Input, fb.Input, c.TEMP_Node);
                var c2 = new EqualConstraint(fa.Output, fb.Output, c.TEMP_Node);

                var cs2 = IConstraintSet.Union(c1, c2, cs);

                return IConstraint.Unify(scope, cs2, tvProvider);
            }

            // They are Type Tuples.
            if (c.A is AzTypeTupleExpn tupa &&
                c.B is AzTypeTupleExpn tupb &&
                (tupa.Elements.Count == tupb.Elements.Count)) 
            {
                ConstraintSet cargs = new();
                for (int i = 0; i < tupa.Elements.Count; i++)
                {
                    var carg = new EqualConstraint(tupa.Elements[i], tupb.Elements[i], c.TEMP_Node);
                    cargs.Add(carg);
                }
                var cs2 = IConstraintSet.Union(cs, cargs);

                return IConstraint.Unify(scope, cs2, tvProvider);
            }

            // They are Type Constructors.
            if (c.A is AzTypeCtorExpn ctora &&
            c.B is AzTypeCtorExpn ctorb &&
            (ctora.TypeDefn == ctorb.TypeDefn) &&
            (ctora.Arguments.Count == ctorb.Arguments.Count))
            {
                ConstraintSet cargs = new();
                for (int i = 0; i < ctora.Arguments.Count; i++)
                {
                    var carg = new EqualConstraint(ctora.Arguments[i], ctorb.Arguments[i], c.TEMP_Node);
                    cargs.Add(carg);
                }
                var cs2 = IConstraintSet.Union(cs, cargs);

                return IConstraint.Unify(scope, cs2, tvProvider);
            }

            throw Analyzer.Error("Type inference failed.", c.TEMP_Node.Position);
        }

        public override string ToString() => string.Format("{0} = {1}", A.Print(0), B.Print(0));
    }
}
