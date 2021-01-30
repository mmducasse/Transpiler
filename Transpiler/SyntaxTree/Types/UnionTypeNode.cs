using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parser;

namespace Transpiler
{
    public interface IUnionTypeNodeMember : IAstNode
    {
    }

    public record UnionTypeNode(IReadOnlyList<IUnionTypeNodeMember> SubTypes) : ITypeExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out UnionTypeNode node)
        {
            node = null;
            int indent = queue.Indent;
            var q = queue;

            var q2 = q;
            if (!Finds(TokenType.NewLine, ref q2)) { return false; }
            if (!FindsIndents(ref q2, indent + 1)) { return false; }

            List<IUnionTypeNodeMember> subTypes = new();
            while (Finds(TokenType.NewLine, ref q) &&
                   FindsIndents(ref q, indent + 1) &&
                   Finds("|", ref q))
            {
                IUnionTypeNodeMember memberNode = null;

                if (TypeDefnNode.Parse(ref q, out var typeDefnNode)) { memberNode = typeDefnNode; }
                else if (TypeSymbolNode.Parse(ref q, out var symbolNode)) { memberNode = symbolNode; }

                if (memberNode != null)
                {
                    subTypes.Add(memberNode);
                }
                else
                {
                    throw Error("Expected union subtype definition or reference after '|'", q);
                }
            }

            node = new(subTypes);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string s = "\n";
            int i1 = i + 1;

            foreach (var t in SubTypes)
            {
                s += string.Format("{0}| {1}\n", Indent(i1), t.Print(i1));
            }

            return s;
        }
    }
}
