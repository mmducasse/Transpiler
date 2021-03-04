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

        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzUndefinedLiteral Analyze(PsUndefinedLiteral psUndefLit)
        {
            return new(psUndefLit.Position);
        }

        public ConstraintSet Constrain() => ConstraintSet.Empty;

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action) { action(this); }

        public string Print(int indent) => "Undefined";
    }
}
