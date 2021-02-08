using Transpiler.Parse;

namespace Transpiler
{
    public interface ITypeExpnNode : IPsNode
    {
        public static bool Parse(ref TokenQueue queue, out ITypeExpnNode node)
        {
            node = null;
            var q = queue;

            if (UnionTypeNode.Parse(ref q, out var unionNode)) { node = unionNode; }
            else if (DataTypeNode.Parse(ref q, out var compNode)) { node = compNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
