namespace Transpiler.Parse
{
    public interface IPsPattern : IPsFuncNode
    {
        public static bool Parse(ref TokenQueue queue, out IPsPattern node)
        {
            node = null;
            var q = queue;

            if (PsElsePattern.Parse(ref q, out var elseNode)) { node = elseNode; }
            else if (PsParam.Parse(ref q, out var parNode)) { node = parNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litNode)) { node = litNode; }
            else if (PsTuplePattern.Parse(ref q, out var tupNode)) { node = tupNode; }
            else if(PsDectorPattern.Parse(ref q, out var dctorNode)) { node = dctorNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
