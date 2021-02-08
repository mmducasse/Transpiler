using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsParam(string Name,
                          CodePosition Position = null) : IPsPattern, IPsFuncDefn
    {
        public eFixity Fixity => eFixity.Prefix;

        public static bool Parse(ref TokenQueue queue, out PsParam node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.Lowercase, ref q, out string symbol))
            {
                node = new PsParam(symbol);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i) => Name;
    }
}
