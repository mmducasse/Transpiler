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
    /// Undefined literal.
    /// </summary>
    public record PsUndefinedLiteral(CodePosition Position) : IPsLiteralExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsUndefinedLiteral node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (Finds("Undefined", ref q))
            {
                node = new(p);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int indent)
        {
            return "Undefined";
        }
    }
}
