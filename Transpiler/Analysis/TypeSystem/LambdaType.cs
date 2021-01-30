using System;

namespace Transpiler
{
    public record LambdaType(IType Input,
                             IType Output) : IType
    {
        public bool IsSolved => Input.IsSolved && Output.IsSolved;

        public static LambdaType Make(params IType[] elements)
        {
            return elements.Length switch
            {
                < 2 => throw new InvalidOperationException(),
                2 => new LambdaType(elements[0], elements[1]),
                _ => new LambdaType(elements[0], LambdaType.Make(elements[1..])),
            };
        }

        public static IType Substitute(LambdaType type, Substitution sub)
        {
            return new LambdaType(IType.Substitute(type.Input, sub),
                                  IType.Substitute(type.Output, sub));
        }

        public static IType Unify(LambdaType t1, LambdaType t2)
        {
            return new LambdaType(IType.Unify(t1.Input, t2.Input),
                                  IType.Unify(t1.Output, t2.Output));
        }

        public string Print()
        {
            return string.Format("{0} -> {1}", Input.Print(), Output.Print());
        }
    }
}
