namespace Transpiler
{
    public static class KeySymbols
    {
        public const string Newline = "\n";
        public const string Return = "\r";
        public const string Tab = "\t";

        public const string LParen = "(";
        public const string RParen = ")";
        public const string LCurly = "{";
        public const string LBrace = "[";
        public const string RBrace = "]";
        public const string Comma = ",";
        public const string Dot = ".";

        public static readonly string[] All = new[]
        {
            LParen,
            RParen,
            LCurly,
            LBrace,
            RBrace,
            Comma,
            Dot,
        };

        public const string Equal = "=";
        public const string Colon = ":";
        public const string Arrow = "->";
        public const string BigArrow = "=>";

        public static readonly string[] ReservedSymbols = new[]
        {
            Equal,
            Colon,
            Arrow,
            BigArrow,
        };
    }
}
