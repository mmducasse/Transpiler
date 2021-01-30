using Transpiler.Parse;

namespace Transpiler
{
    public interface IPatternNode : IFuncNode
    {
        public static bool Parse(ref TokenQueue queue, out IPatternNode node)
        {
            node = null;
            var q = queue;

            if (DeconstructPatternNode.Parse(ref q, out var dctorNode)) { node = dctorNode; }
            // Tuple?
            else if (SymbolNode.Parse(ref q, out var varNode)) { node = varNode; }
            else if (ILiteralNode.Parse(ref q, out var litNode)) { node = litNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
