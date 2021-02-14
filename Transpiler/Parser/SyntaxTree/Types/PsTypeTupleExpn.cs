using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTypeTupleExpn(IReadOnlyList<IPsTypeExpn> Elements,
                                  CodePosition Position) : IPsTypeExpn
    {
        public static PsTypeTupleExpn Empty => new(new List<IPsTypeSymbolExpn>(), CodePosition.Null);

        public static bool Parse(ref TokenQueue queue, out IPsTypeExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            List<IPsTypeExpn> members = new();

            if (!PsTypeArbExpn.Parse(ref q, out var first)) { return false; }

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
                if (!PsTypeArbExpn.Parse(ref q, out var next))
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
            if (Elements.Count == 0)
            {
                return "()";
            }

            string members = Elements.Select(m => m.Print(i)).Separate(", ");
            return string.Format("{0}", members);
        }

        public override string ToString() => Print(0);
    }
}
