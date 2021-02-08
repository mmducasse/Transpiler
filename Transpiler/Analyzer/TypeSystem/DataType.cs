using System.Collections.Generic;

namespace Transpiler.Analysis
{
    //public record DataTypeMember(string Name,
    //                             IType Type);

    /// <summary>
    /// Named type consisting of zero or more member values.
    /// </summary>
    public record DataType(string Name,
                           IReadOnlyList<string> Elements) : INamedType
    {
        public bool IsSolved => true;

        public static DataType Make(string name, params string[] elements) =>
            new(name, elements);

        public string Print(bool terse = true)
        {
            var elements = (Elements.Count > 0)
                ? Elements.Separate(", ")
                : "()";
            return string.Format("{0} = {1}", Name, elements);
        }
    }
}
