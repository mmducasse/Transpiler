using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parser;

namespace Transpiler
{
    public record MatchNode(IFuncExpnNode Argument,
                            IReadOnlyList<MatchCaseNode> Cases) : IFuncExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out MatchNode node)
        {
            node = null;
            int indent = queue.Indent;
            var q = queue;

            if (!Finds("match", ref q)) { return false; }
            if (!ArbitraryExpnNode.Parse(ref q, out var conditionNode))
            {
                throw Error("Expected inline expression after 'match'", q);
            }

            var q2 = q;
            if (!Finds(TokenType.NewLine, ref q2)) { return false; }
            if (!FindsIndents(ref q2, indent + 1)) { return false; }
            if (!Finds("|", ref q2)) { return false; }

            List<MatchCaseNode> cases = new();
            while (Finds(TokenType.NewLine, ref q) &&
                   FindsIndents(ref q, indent + 1) &&
                   Finds("|", ref q))
            {
                if (!MatchCaseNode.Parse(ref q, out var matchCaseNode))
                {
                    throw Error("Expected pattern match case after '|'", q);
                }

                cases.Add(matchCaseNode);
            }

            node = new MatchNode(conditionNode, cases);
            queue = q;
            return true;
        }


        public string Print(int i)
        {
            string s = string.Format("match {0}\n", Argument.Print(i));
            foreach (var c in Cases)
            {
                s += string.Format("{0}| {1}\n", Indent(i + 1), c.Print(i + 1));
            }

            return s;
        }
    }
}
