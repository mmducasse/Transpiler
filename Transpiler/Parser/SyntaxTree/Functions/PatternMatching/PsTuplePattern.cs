using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTuplePattern(IReadOnlyList<IPsPattern> Elements,
                                 CodePosition Position) : IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out PsTuplePattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            List<IPsPattern> elements = new();
            if (!ParseSubPattern(ref q, out var first)) { return false; }
            elements.Add(first);
            while (Finds(",", ref q))
            {
                var pn = q.Position;
                if (!ParseSubPattern(ref q, out var next))
                {
                    throw Error("Expected pattern after ','", pn);
                }

                elements.Add(next);
            }

            if (elements.Count < 2)
            {
                return false;
            }

            node = new PsTuplePattern(elements, p);
            queue = q;

            return true;
        }

        public static bool ParseSubPattern(ref TokenQueue queue,
                                            out IPsPattern node)
        {
            node = null;
            var q = queue;

            bool inParens = false;
            if (Finds("(", ref q))
            {
                inParens = true;
            }

            if (inParens && PsTuplePattern.Parse(ref q, out var tupNode)) { node = tupNode; }
            else if (PsDectorPattern.Parse(ref q, out var dctorNode)) { node = dctorNode; }
            else if (PsAnyPattern.Parse(ref q, out var elseNode)) { node = elseNode; }
            else if (PsParam.Parse(ref q, out var parNode)) { node = parNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litExpnNode)) { node = litExpnNode; }

            if (node != null)
            {
                if (inParens)
                {
                    Expects(")", ref q);
                }
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i)
        {
            var es = Elements.Select(v => v.Print(i)).Separate(", ");
            return string.Format("{0}", es);
        }

        public override string ToString() => Print(0);
    }
}
