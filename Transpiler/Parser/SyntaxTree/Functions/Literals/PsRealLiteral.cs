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
    /// Real number literal.
    /// </summary>
    public record PsRealLiteral(string Value,
                                CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsRealLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.NumberLiteral, ref q, out string whole) &&
                Finds(".", ref q) &&
                Finds(TokenType.NumberLiteral, ref q, out string frac))
            {
                string real = whole + "." + frac;
                real = real.Replace("_", "");

                node = new PsRealLiteral(real, p);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int indent)
        {
            //return ((int)Value == Value)
            //    ? string.Format("{0:0.0}", Value)
            //    : Value.ToString();
            return Value;
        }
    }
}
