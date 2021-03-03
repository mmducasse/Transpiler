using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzIntLiteral(string Value,
                               CodePosition Position) : IAzLiteralExpn
    {
        public IAzTypeDefn CertainType => Core.Instance.Int;

        public IAzTypeExpn Type => Core.Instance.Int.ToCtor();

        public static AzIntLiteral Analyze(PsIntLiteral psIntLit)
        {
            return new(psIntLit.Value, psIntLit.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public void SubstituteType(Substitution s) { }

        public void Recurse(Action<IAzFuncNode> action) { action(this); }

        public string Print(int indent) => Value;
    }
}
