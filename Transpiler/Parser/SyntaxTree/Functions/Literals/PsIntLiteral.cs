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
    /// Integer literal.
    /// </summary>
    public record PsIntLiteral(string Value,
                               CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsIntLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.NumberLiteral, ref q, out string value))
            {
                value = value.Replace("_", "");

                node = new PsIntLiteral(value, p);
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
