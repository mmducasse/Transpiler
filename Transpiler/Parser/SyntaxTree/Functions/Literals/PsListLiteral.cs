using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsListLiteral(IReadOnlyList<IPsFuncExpn> Elements,
                                CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsListLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds("[", ref q))
            {
                List<IPsFuncExpn> elements = new();
                while (!Finds(TokenType.NewLine, ref q))
                {
                    if (!PsArbExpn.Parse(ref q, isInline: true, out var arbNode))
                    {
                        throw Error("Expected expression in list element.", q.Position);
                    }
                    elements.Add(arbNode);
                    if (Finds("]", ref q))
                    {
                        node = new PsListLiteral(elements, p);
                        queue = q;
                        return true;
                    }
                    Expects(",", ref q);
                }
            }

            return false;
        }

        public string Print(int indent)
        {
            var elements = Elements.Select(e => e.Print(0)).Separate(", ");
            return string.Format("[{0}]", elements);
        }
    }
}
