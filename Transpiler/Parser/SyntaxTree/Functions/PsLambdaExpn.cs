using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsLambdaExpn(PsParam Parameter,
                               IPsFuncExpn Expression,
                               CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsLambdaExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!PsParam.Parse(ref q, out var paramNode)) { return false; }
            if (!Finds("->", ref q)) { return false; }
            if (!IPsFuncExpn.Parse(ref q, out var expnNode))
            {
                throw Error("Expected expression after '->' in lambda function.", q);
            }

            node = new PsLambdaExpn(paramNode, expnNode, p);
            queue = q;
            return true;
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Parameter.Print(indent + 1),
                                 Expression.Print(indent + 1));
        }
    }
}
