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
            if (!PsDectorPattern.Parse(ref q, out var first)) { return false; }
            elements.Add(first);
            while (Finds(",", ref q))
            {
                var pn = q.Position;
                if (!PsDectorPattern.Parse(ref q, out var next))
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

        public string Print(int i)
        {
            var es = Elements.Select(v => v.Print(i)).Separate(", ");
            return string.Format("{0}", es);
        }

        public override string ToString() => Print(0);
    }
}
