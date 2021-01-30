using System.Collections.Generic;
using static Transpiler.Extensions;

namespace Transpiler
{
    public record UnionType(string Name,
                            IReadOnlyList<string> Elements) : ICompositeType
    {
        public bool IsSolved => true;

        public static UnionType Make(string name, params string[] elements) =>
            new(name, elements);

        public string Print()
        {
            string s = Name + " =\n";
            foreach (var e in Elements)
            {
                s += string.Format("{0}| {1}\n", Indent(1), e);
            }

            return s;
        }
    }
}
