using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzElsePattern(CodePosition Position) : IAzPattern
    {
        public static AzElsePattern Analyze(Scope scope,
                                            PsElsePattern node)
        {
             return new(node.Position);
        }

        public string Print(int i) => "_";

        public override string ToString() => Print(0);
    }
}
