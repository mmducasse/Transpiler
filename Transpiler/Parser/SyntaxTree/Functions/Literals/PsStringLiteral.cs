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
    /// String literal.
    /// </summary>
    public record PsStringLiteral(string Value,
                                  CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsStringLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.DoubleQuoted, ref q, out string value))
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
