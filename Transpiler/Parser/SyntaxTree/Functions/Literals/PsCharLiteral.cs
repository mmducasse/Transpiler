// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// Character literal.
    /// </summary>
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
