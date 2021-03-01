using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzUndefinedLiteral(CodePosition Position) : IAzLiteralExpn
    {
        public string Value => "Undefined";

        private static TypeVariable mCertainType = TypeVariable.Simple(0);
        public IAzTypeDefn CertainType => mCertainType;

        public IAzTypeExpn Type => mCertainType;

        public static AzUndefinedLiteral Analyze(PsUndefinedLiteral psUndefLit)
        {
            return new(psUndefLit.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IAzFuncExpn SubstituteType(Substitution s) => this;

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int indent) => "Undefined";

        IAzPattern IAzPattern.SubstituteType(Substitution s)
        {
            throw new NotImplementedException();
        }
    }
}
