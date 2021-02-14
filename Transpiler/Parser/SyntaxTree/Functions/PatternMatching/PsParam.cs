using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsParam(string Name,
                          bool IsWildcard = false,
                          CodePosition Position = null) : IPsPattern, IPsFuncDefn
    {
        public eFixity Fixity => eFixity.Prefix;

        public static bool Parse(ref TokenQueue queue, out PsParam node)
        {
            node = null;
            var q = queue;

            if (Finds("_", ref q))
            {
                node = new PsParam("_", IsWildcard: true);
                queue = q;
                return true;
            }

            if (Finds(TokenType.Lowercase, ref q, out string symbol))
            {
                node = new PsParam(symbol);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i) => Name;

        public override string ToString() => Print(0);
    }
}
