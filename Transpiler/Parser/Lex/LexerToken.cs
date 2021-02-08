
namespace Transpiler.Parse
{
    public enum TokenType
    {
        NumberLiteral =   1,
        Lowercase     =   2,
        Uppercase     =   4,
        Alphabetic    =   Lowercase | Uppercase,
        KeySymbol     =   8,
        Symbol        =  16,
        Name          =   Symbol | Alphabetic,
        SingleQuoted  =  32,
        DoubleQuoted  =  64,
        Indent        = 128,
        NewLine       = 256
    }

    public class LexerToken
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        public CodePosition Position { get; set; }

        public LexerToken() { }

        public LexerToken(TokenType type)
        {
            this.Type = type;
        }

        public override string ToString()
        {
            string s = "";

            s += string.Format("({0:D2}, {1:D2}) ", Position.Line / 2, Position.Column);

            s += Type switch
            {
                TokenType.NumberLiteral => "NUM ",
                TokenType.Lowercase => "LOW ",
                TokenType.Uppercase => "UPR ",
                TokenType.Symbol => "SYM ",
                TokenType.KeySymbol => "KEY ",
                TokenType.SingleQuoted => "SINQ",
                TokenType.DoubleQuoted => "DUBQ",
                TokenType.Indent => "INDT",
                TokenType.NewLine => "NEWL",
                _ => "???"
            };

            s += ": ";
            s += Value;
            return s;
        }
    }
}