using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzCharLiteral(string Value,
                                CodePosition Position) : IAzLiteralExpn
    {
        public IAzTypeDefn CertainType => Core.Instance.Char;

        public IAzTypeExpn Type => Core.Instance.Char.ToCtor();

        public static AzCharLiteral Analyze(PsCharLiteral psCharLit)
        {
            return new(psCharLit.Value, psCharLit.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public void SubstituteType(Substitution s) { }

        public void Recurse(Action<IAzFuncNode> action) { action(this); }

        public string Print(int indent)
        {
            return string.Format("'{0}'", Value);
        }

        public override string ToString() => Print(0);
    }
}
