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

        public ConstraintSet Constrain() => ConstraintSet.Empty;

        public void SubstituteType(Substitution s) { }

        public void Recurse(Action<IAzFuncNode> action) { action(this); }

        public string Print(int indent) => Value;
    }
}
