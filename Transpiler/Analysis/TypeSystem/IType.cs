using System;

namespace Transpiler
{
    public interface IType
    {
        bool IsSolved { get; }

        string Print(bool terse = true);

        public static Substitution Unify(ConstraintSet set)
        {
            if (set.IsEmpty)
            {
                return new Substitution();
            }

            var (c, cs) = set.Next;

            // Equal types.
            if (c.A == c.B)
            {
                return Unify(cs);
            }

            // A is a Type Variable.
            if (c.A is TypeVariable tva && !Contains(c.B, tva))
            {
                var s = new Substitution(tva, c.B);
                var s2 = Unify(cs.Substitute(s));

                return new Substitution(s2, s);
            }

            // B is a Type Variable.
            if (c.B is TypeVariable tvb && !Contains(c.A, tvb))
            {
                var s = new Substitution(tvb, c.A);
                var s2 = Unify(cs.Substitute(s));

                return new Substitution(s2, s);
            }

            // They are Function Types.
            if (c.A is FunType fa && c.B is FunType fb)
            {
                var c1 = new Constraint(fa.Input, fb.Input, null);
                var c2 = new Constraint(fa.Output, fb.Output, null);

                var cs2 = IConstraints.Union(c1, c2, cs);

                return Unify(cs2);
            }

            // Add case for Tuple
            // Ad case for Type ctor.

            throw TypeSolver.Error("Type inference failed.", c.TEMP_Node);
        }

        private static bool Contains(IType t, TypeVariable tv)
        {
            return t switch
            {
                TypeVariable tv2 => tv.Equals(tv2),
                FunType ft => Contains(ft.Input, tv) || Contains(ft.Output, tv),
                // Todo: Add tuple case.
                _ => false,
            };
        }

        public static IType Substitute(IType type, Substitution sub)
        {
            if (type.IsSolved) { return type; }

            if (type is TypeVariable tv &&
                sub.TypeSubstitutions.ContainsKey(tv))
            {
                return Substitute(sub.TypeSubstitutions[tv], sub);
            }

            return type switch
            {
                FunType lamType => FunType.Substitute(lamType, sub),
                _ => type,
            };
        }
    }

    public interface INamedType : IType
    {
        string Name { get; }
    }
}
