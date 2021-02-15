using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsElsePattern(CodePosition Position) : IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out PsElsePattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds("_", ref q)) { return false; }

            node = new PsElsePattern(p);
            queue = q;

            return true;
        }

        public string Print(int i) => "_";

        public override string ToString() => Print(0);
    }
}
