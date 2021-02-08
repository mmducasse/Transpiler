using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTypeTupleExpn(IReadOnlyList<PsTypeSymbol> Members,
                                  CodePosition Position) : IPsTypeExpn
    {
        public static bool Parse(ref TokenQueue queue, out IPsTypeExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            List<PsTypeSymbol> members = new();

            if (!IPsTypeSymbol.Parse(ref q, out var first)) { return false; }

            var q2 = q;
            if (!Finds(",", ref q2))
            {
                node = first;
                queue = q;
                return true;
            }

            members.Add(first);
            while (Finds(",", ref q))
            {
                if (!IPsTypeSymbol.Parse(ref q, out var next))
                {
                    throw Error("Expected simple type expression after ','.", q);
                }
                members.Add(next);
            }

            node = new PsTypeTupleExpn(members, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string members = Members.Select(m => m.Print(i)).Separate(", ");
            return string.Format("{0}", members);
        }
    }
}
