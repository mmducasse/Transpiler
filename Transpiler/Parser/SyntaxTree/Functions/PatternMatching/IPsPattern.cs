namespace Transpiler.Parse
{
    public interface IPsPattern : IPsFuncNode
    {
        public static bool Parse(ref TokenQueue queue, out IPsPattern node)
        {
            node = null;
            var q = queue;

            //if (DeconstructPatternNode.Parse(ref q, out var dctorNode)) { node = dctorNode; }
            // Tuple?
            if (PsSymbolExpn.Parse(ref q, out var varNode)) { node = varNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litNode)) { node = litNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
