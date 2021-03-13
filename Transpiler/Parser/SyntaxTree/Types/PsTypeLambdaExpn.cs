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
    /// A lambda type expression.
    /// </summary>
    public record PsTypeLambdaExpn(IPsTypeExpn Input,
                                IPsTypeExpn Output,
                                CodePosition Position) : IPsTypeExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsTypeLambdaExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!IPsTypeSymbolExpn.Parse(ref q, out var inNode)) { return false; }
            if (!Finds("->", ref q)) { return false; }
            if (!IPsTypeExpn.Parse(ref q, out var outNode))
            {
                throw Error("Expected type expression after '->' in function type expression.", q);
            }

            node = new PsTypeLambdaExpn(inNode, outNode, p);
            queue = q;
            return true;
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Input.Print(indent + 1),
                                 Output.Print(indent + 1));
        }
    }
}
