﻿using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzCharLiteral(string Value,
                                CodePosition Position) : IAzLiteralExpn
    {
        public IAzTypeDefn CertainType => Core.Instance.Char;

        public IAzTypeExpn Type
        {
            get => Core.Instance.Char.ToCtor();
            set
            {
                if (value != CertainType)
                {
                    //throw new Exception();
                }
            }
        }

        public static AzCharLiteral Analyze(Scope scope,
                                            NameProvider provider,
                                            PsCharLiteral node)
        {
            return new(node.Value, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IReadOnlyList<IAzFuncNode> GetSubnodes() => this.ToArr();

        public string Print(int indent)
        {
            return string.Format("'{0}'", Value);
        }

        public override string ToString() => Print(0);
    }
}
