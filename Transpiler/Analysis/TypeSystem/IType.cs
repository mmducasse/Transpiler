using System;

namespace Transpiler
{
    public interface IType
    {
        bool IsSolved { get; }

        string Print();

        public static IType Substitute(IType type, Substitution sub)
        {
            if (type == sub.OldType)
            {
                return sub.NewType;
            }

            return type switch
            {
                LambdaType lamType => LambdaType.Substitute(lamType, sub),
                _ => type,
            };
        }

        public static IType Unify(IType t1, IType t2)
        {
            return (t1, t2) switch
            {
                (LambdaType l1, LambdaType l2) => LambdaType.Unify(l1, l2),
                (INamedType n1, INamedType n2) => INamedType.Unify(n1, n2),
                (_, TypeVariable tv) => t1,
                (TypeVariable tv, _) => t2,
                _ => throw new InvalidOperationException(),
            };
        }
    }

    public interface INamedType : IType
    {
        string Name { get; }

        public static IType Unify(INamedType t1, INamedType t2)
        {
            if (t1.Name == t2.Name)
            {
                return t1;
            }

            throw new InvalidOperationException();
        }
    }
}
