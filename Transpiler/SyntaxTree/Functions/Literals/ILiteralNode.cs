using Transpiler.Parse;

namespace Transpiler
{
    public interface ILiteralNode : IFuncExpnNode, IPatternNode
    {
        IType CertainType { get; }

        public static bool Parse(ref TokenQueue queue, out ILiteralNode node)
        {
            node = null;
            var q = queue;

            if (RealNode.Parse(ref q, out var realNode)) { node = realNode; }
            else if (IntNode.Parse(ref q, out var intNode)) { node = intNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }

    public interface ILiteralNode<T> : ILiteralNode
    {
        T Value { get; }
    }
}
