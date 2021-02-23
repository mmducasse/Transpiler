namespace Transpiler.Parse
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
        //public const string Dot = ".";

        public static readonly string[] All = new[]
        {
            LParen,
            RParen,
            LCurly,
            LBrace,
            RBrace,
            Comma,
            //Dot,
        };

        public const string Equal = "=";
        public const string Colon = ":";
        public const string IReadOnlyListow = "->";
        public const string BigIReadOnlyListow = "=>";

        public static readonly string[] ReservedSymbols = new[]
        {
            Equal,
            Colon,
            IReadOnlyListow,
            BigIReadOnlyListow,
        };

        public const string If = "if";
        public const string Then = "then";
        public const string Else = "else";
        public const string Match = "match";
        //public const string Type = "type";
        //public const string Inst = "inst";

        public static readonly string[] ReservedWords = new[]
        {
            If,
            Then,
            Else,
            Match,
            //Type,
            //Inst,
        };
    }
}
