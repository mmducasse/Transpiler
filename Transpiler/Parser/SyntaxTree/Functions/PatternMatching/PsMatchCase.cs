using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsMatchCase(IPsPattern Pattern,
                                IPsFuncExpn Expression)
    {
        public static bool Parse(ref TokenQueue queue, out PsMatchCase node)
        {
            node = null;
            var q = queue;

            if (!IPsPattern.Parse(ref q, out var pattNode)) { return false; }
            Expects("->", ref q);
            if (!PsScopedFuncExpn.Parse(ref q, out var expnNode))
            {
                throw Error("Expected expression after '->' in match case.", q);
            }

            node = new PsMatchCase(pattNode, expnNode);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), Expression.Print(i + 1));
        }
    }
}
