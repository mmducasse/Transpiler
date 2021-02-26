using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTypeRefinement(string TypeClassName,
                                   string TypeVarName,
                                   CodePosition Position) : IPsNode
    {
        public static bool Parse(ref TokenQueue queue, out PsTypeRefinement node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.Uppercase, ref q, out string className) &&
                Finds(TokenType.Lowercase, ref q, out string varName))
            {
                node = new PsTypeRefinement(className, varName, p);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i) => string.Format("{0} {1}", TypeClassName, TypeVarName);

        public override string ToString() => Print(0);
    }

    public record PsTypeRefinementGroup(IReadOnlyList<PsTypeRefinement> Refinements,
                                        CodePosition Position) : IPsNode
    {
        public static PsTypeRefinementGroup Empty =>
            new(new List<PsTypeRefinement>(), CodePosition.Null);

        public static bool Parse(ref TokenQueue queue, out PsTypeRefinementGroup node)
        {
            node = Empty;
            var q = queue;
            var p = q.Position;

            List<PsTypeRefinement> refinements = new();
            while (PsTypeRefinement.Parse(ref q, out var r))
            {
                refinements.Add(r);
                if (Finds("=>", ref q))
                {
                    node = new PsTypeRefinementGroup(refinements, p);
                    queue = q;
                    return true;
                }
                else if (!Finds(",", ref q))
                {
                    return false;
                }
            }

            return false;
        }

        public string Print(int i)
        {
            if (Refinements.Count > 0)
            {
                string rs = Refinements.Select(r => r.Print(0)).Separate(", ");
                return string.Format("{0} => ", rs);
            }

            return "";
        }

        public override string ToString() => Print(0);
    }
}
