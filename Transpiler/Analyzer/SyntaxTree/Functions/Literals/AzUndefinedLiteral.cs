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

        public IAzTypeExpn Type
        {
            get => mCertainType;
            set
            {
                if (value != CertainType)
                {
                    //throw new Exception();
                }
            }
        }

        public static AzUndefinedLiteral Analyze(Scope scope,
                                                 NameProvider provider,
                                                 PsUndefinedLiteral node)
        {
            return new(node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int indent) => "Undefined";
    }
}
