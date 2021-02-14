using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public record TypeVariable(int Id,
                               IReadOnlyList<AzClassTypeDefn> Refinements) : IAzTypeDefn, IAzTypeExpn
    {
        public bool IsSolved => false;

        public string Name => "t" + Id.ToString();

        public CodePosition Position => CodePosition.Null;

        public bool HasRefinements => (Refinements?.Count() ?? 0) > 0;

        public static TypeVariable Simple(int id) => new(id, new List<AzClassTypeDefn>());

        public ISet<TypeVariable> GetTypeVars()
        {
            return new HashSet<TypeVariable> { this };
        }

        public string Print(bool terse = true) => string.Format("t{0}", Id);

        public string Print(int indent)
        {
            return Name;
        }

        public string PrintRefinements()
        {
            if (Refinements.Count > 0)
            {
                return Refinements.Select(r => r.Name + " " + Print()).Separate(", ");
            }

            return string.Empty;
        }

        public override string ToString() => Print(0);
    }
}
