using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTypeFunExpn(IPsTypeExpn Input,
                                IPsTypeExpn Output,
                                CodePosition Position) : IPsTypeExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsTypeFunExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!IPsTypeSymbol.Parse(ref q, out var inNode)) { return false; }
            if (!Finds("->", ref q)) { return false; }
            if (!IPsTypeExpn.Parse(ref q, out var outNode))
            {
                throw Error("Expected type expression after '->' in function type expression.", q);
            }

            node = new PsTypeFunExpn(inNode, outNode, p);
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
