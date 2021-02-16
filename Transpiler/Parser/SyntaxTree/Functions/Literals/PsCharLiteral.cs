using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsCharLiteral(string Value,
                                CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsCharLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.SingleQuoted, ref q, out string value))
            {
                if (value.Length != 1)
                {
                    throw Error("Single quotes must contain a single character.", p);
                }

                node = new(value, p);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int indent)
        {
            return Value.ToString();
        }
    }
}
