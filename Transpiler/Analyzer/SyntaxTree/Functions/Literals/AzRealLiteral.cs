using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzRealLiteral(string Value,
                                CodePosition Position) : IAzLiteralExpn
    {
        public IAzTypeDefn CertainType => Core.Instance.Real;

        public IAzTypeExpn Type => Core.Instance.Real.ToCtor();

        public static AzRealLiteral Analyze(PsRealLiteral psRealNode)
        {
            return new(psRealNode.Value, psRealNode.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IAzFuncExpn SubstituteType(Substitution s) => this;
        IAzPattern IAzPattern.SubstituteType(Substitution s) => this;

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int indent) => Value;
    }
}
