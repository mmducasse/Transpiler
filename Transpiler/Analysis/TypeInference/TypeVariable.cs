using System.Collections.Generic;
using System.Linq;

namespace Transpiler
{
    public record TypeVariable(int Id,
                               IReadOnlyList<IClassType> Refinements) : IType
    {
        public bool IsSolved => false;

        public bool HasRefinements => (Refinements?.Count() ?? 0) > 0;

        public TypeVariable MadeUnique => this with { Id = mTvIndex++ };

        private static int mTvIndex = 10;

        public static TypeVariable Next => new(mTvIndex++, new List<IClassType>());

        public static TypeVariable NextR(params IClassType[] refinements) =>
            new(mTvIndex++, refinements);

        public string Print(bool terse = true) => string.Format("t{0}", Id);

        public string PrintRefinements()
        {
            if (Refinements.Count > 0)
            {
                return Refinements.Select(r => r.Name + " " + Print()).Separate(", ");
            }

            return string.Empty;
        }
    }
}
