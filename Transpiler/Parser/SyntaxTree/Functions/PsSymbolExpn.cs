using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsSymbolExpn(string Name,
                               CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsSymbolExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.Alphabetic | TokenType.Symbol, ref q, out string symbol))
            {
                node = new PsSymbolExpn(symbol, p);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int indent)
        {
            return Name;
        }
    }
}
