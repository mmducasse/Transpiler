using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsIntLiteral(int Value,
                               CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsIntLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.NumberLiteral, ref q, out string value))
            {
                if (int.TryParse(value, out int i))
                {
                    node = new PsIntLiteral(i, p);
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
