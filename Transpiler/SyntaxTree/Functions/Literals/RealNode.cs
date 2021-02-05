using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record RealNode(double Value) : ILiteralNode<double>
    {
        public IType CertainType => CoreTypes.Instance.Num;

        public static bool Parse(ref TokenQueue queue, out RealNode node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.NumberLiteral, ref q, out string whole) &&
                Finds(".", ref q) &&
                Finds(TokenType.NumberLiteral, ref q, out string frac))
            {
                string real = whole + "." + frac;
                if (double.TryParse(real, out double d))
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
