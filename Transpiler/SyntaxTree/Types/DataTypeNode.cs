using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record DataTypeNode(IReadOnlyList<TypeSymbolNode> Members) : ITypeExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out ITypeExpnNode node)
        {
            node = null;
            var q = queue;
            List<TypeSymbolNode> members = new();

            if (!TypeSymbolNode.Parse(ref q, out var first)) { return false; }

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
                if (!TypeSymbolNode.Parse(ref q, out var next))
                {
                    throw Error("Expected simple type expression after ','.", q);
                }
                members.Add(next);
            }

            node = new DataTypeNode(members);
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
