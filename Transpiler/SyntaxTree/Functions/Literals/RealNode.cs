using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record RealNode(double Value) : ILiteralNode<double>
    {
        public IType CertainType => CoreTypes.Instance.Real;

        public static bool Parse(ref TokenQueue queue, out RealNode node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.NumberLiteral, ref q, out string value))
            {
                if (double.TryParse(value, out double d))
                {
                    node = new RealNode(d);
                    queue = q;
                    return true;
                }
            }

            return false;
        }

        public string Print(int indent)
        {
            return ((int)Value == Value)
                ? string.Format("{0:0.0}", Value)
                : Value.ToString();
        }
    }
}
