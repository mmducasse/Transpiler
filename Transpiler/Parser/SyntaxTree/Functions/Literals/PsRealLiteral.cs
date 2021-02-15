using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsRealLiteral(double Value,
                                CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsRealLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.NumberLiteral, ref q, out string whole) &&
                Finds(".", ref q) &&
                Finds(TokenType.NumberLiteral, ref q, out string frac))
            {
                string real = whole + "." + frac;
                if (double.TryParse(real, out double d))
                {
                    node = new PsRealLiteral(d, p);
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
