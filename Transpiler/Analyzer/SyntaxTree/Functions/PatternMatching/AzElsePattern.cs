using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzElsePattern(CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; set; }

        public static AzElsePattern Analyze(Scope scope,
                                            PsAnyPattern node)
        {
             return new(node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            return ConstraintSet.Empty;
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int i) => "_";

        public override string ToString() => Print(0);
    }
}
