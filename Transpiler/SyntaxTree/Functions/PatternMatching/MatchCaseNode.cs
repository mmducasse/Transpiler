using System;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record MatchCaseNode(IPatternNode Pattern,
                                IFuncExpnNode Expression)
    {
        public static bool Parse(ref TokenQueue queue, out MatchCaseNode node)
        {
            node = null;
            var q = queue;

            if (!IPatternNode.Parse(ref q, out var pattNode)) { return false; }
            Expects("->", ref q);
            if (!ScopedFuncExpnNode.Parse(ref q, out var expnNode))
            {
                throw Error("Expected expression after '->' in match case.", q);
            }

            node = new MatchCaseNode(pattNode, expnNode);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), Expression.Print(i + 1));
        }
    }
}
