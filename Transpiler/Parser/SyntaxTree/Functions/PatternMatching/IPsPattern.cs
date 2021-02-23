using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public interface IPsPattern : IPsFuncNode
    {
        public static bool Parse(ref TokenQueue queue,
                                 out IPsPattern node)
        {
            node = null;
            var q = queue;

            if (PsTuplePattern.Parse(ref q, out var tupNode)) { node = tupNode; }
            else if (PsDectorPattern.Parse(ref q, out var dctorNode)) { node = dctorNode; }
            else if (PsAnyPattern.Parse(ref q, out var elseNode)) { node = elseNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litExpnNode)) { node = litExpnNode; }
            else if (PsArbExpn.Parse(ref q, isInline: true, out var arbExpnNode)) { node = arbExpnNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
