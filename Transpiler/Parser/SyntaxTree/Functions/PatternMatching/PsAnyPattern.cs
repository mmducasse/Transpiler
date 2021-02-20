using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsAnyPattern(CodePosition Position) : IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out PsAnyPattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds("_", ref q)) { return false; }

            node = new PsAnyPattern(p);
            queue = q;

            return true;
        }

        public string Print(int i) => "_";

        public override string ToString() => Print(0);
    }
}
