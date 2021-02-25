using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzIntLiteral(string Value,
                               CodePosition Position) : IAzLiteralExpn
    {
        public IAzTypeDefn CertainType => Core.Instance.Int;

        public IAzTypeExpn Type
        {
            get => Core.Instance.Int.ToCtor();
            set
            {
                if (value != CertainType)
                {
                    //throw new Exception();
                }
            }
        }

        public static AzIntLiteral Analyze(Scope scope,
                                           PsIntLiteral node)
        {
            return new(node.Value, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int indent) => Value;
    }
}
