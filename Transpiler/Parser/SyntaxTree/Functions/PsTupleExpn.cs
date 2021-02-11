using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTupleExpn(IReadOnlyList<IPsFuncExpn> Elements,
                              CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsTupleExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            var subExpns = new List<IPsFuncExpn>();

            if (!PsArbExpn.Parse(ref q, out var firstExpn)) { return false; }
            var q2 = q;
            if (!Finds(",", ref q2)) { return false; }
            subExpns.Add(firstExpn);
            while (Finds(",", ref q))
            {
                if (!PsArbExpn.Parse(ref q, out var nextExpn))
                {
                    throw Error("Expected expression after ',' in tuple.", q);
                }

                subExpns.Add(nextExpn);
            }

            node = new PsTupleExpn(subExpns, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return Elements.Select(m => m.Print(i)).Separate(", ");
        }
    }
}
