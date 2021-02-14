namespace Transpiler.Parse
{
    public interface IPsPattern : IPsFuncNode
    {
        public static bool Parse(ref TokenQueue queue, out IPsPattern node)
        {
            node = null;
            var q = queue;

            // Tuple?
            if (PsParam.Parse(ref q, out var parNode)) { node = parNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litNode)) { node = litNode; }
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
