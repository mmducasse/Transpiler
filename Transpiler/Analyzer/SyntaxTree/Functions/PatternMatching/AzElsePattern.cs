using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzElsePattern(CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzElsePattern Analyze(PsAnyPattern node)
        {
            return new(node.Position);
        }

        public ConstraintSet Constrain(TvProvider tvs, Scope scope) => ConstraintSet.Empty;

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            action(this);
        }

        public string Print(int i) => "_";

        public override string ToString() => Print(0);
    }
}
