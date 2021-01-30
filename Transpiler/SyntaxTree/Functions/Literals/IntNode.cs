using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record IntNode(int Value) : ILiteralNode<int>
    {
        public IType CertainType => CoreTypes.Instance.Int;

        public static bool Parse(ref TokenQueue queue, out IntNode node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.NumberLiteral, ref q, out string value))
            {
                if (int.TryParse(value, out int i))
                {
                    node = new IntNode(i);
                    queue = q;
                    return true;
                }
            }

            return false;
        }

        public string Print(int indent)
        {
            return Value.ToString();
        }
    }
}
