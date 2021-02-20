using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzRealLiteral(string Value,
                                CodePosition Position) : IAzLiteralExpn
    {
        public IAzDataTypeDefn CertainType => CoreTypes.Instance.Real;

        public IAzTypeExpn Type
        {
            get => CertainType.ToCtor();
            set
            {
                if (value != CertainType)
                {
                    //throw new Exception();
                }
            }
        }

        public static AzRealLiteral Analyze(Scope scope,
                                            PsRealLiteral node)
        {
            return new(node.Value, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int indent) => Value;
    }
}
