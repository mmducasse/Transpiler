using System;

namespace Transpiler.Analysis
{
    public record FunType(IType Input,
                          IType Output) : ICompositeType
    {
        public bool IsSolved => Input.IsSolved && Output.IsSolved;

        public IType Left => Input;
        public IType Right => Output;

        public static FunType Make(params IType[] elements)
        {
            return elements.Length switch
            {
                < 2 => throw new InvalidOperationException(),
                2 => new FunType(elements[0], elements[1]),
                _ => new FunType(elements[0], FunType.Make(elements[1..])),
            };
        }

        public static IType Substitute(FunType type, Substitution sub)
        {
            return new FunType(IType.Substitute(type.Input, sub),
                               IType.Substitute(type.Output, sub));
        }

        public string Print(bool terse = true)
        {
            return string.Format("{0} -> {1}", Input.Print(terse), Output.Print(terse));
        }
    }
}
