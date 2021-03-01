using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzElsePattern(IAzTypeExpn Type,
                                CodePosition Position) : IAzPattern
    {
        public static AzElsePattern Analyze(TvProvider tvs,
                                            PsAnyPattern node)
        {
             return new(tvs.Next, node.Position);
        }

        public ConstraintSet Constrain(TvProvider tvs, Scope scope) => ConstraintSet.Empty;

        public IAzPattern SubstituteType(Substitution s)
        {
            return this with { Type = Type.Substitute(s) };
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int i) => "_";

        public override string ToString() => Print(0);
    }
}
