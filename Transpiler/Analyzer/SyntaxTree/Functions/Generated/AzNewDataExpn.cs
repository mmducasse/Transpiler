﻿using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public record AzNewDataExpn(AzDataTypeDefn Definition) : IAzFuncExpn
    {
        public IReadOnlyList<AzSymbolExpn> Arguments { get; set; }

        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn Type { get; set; }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr();
        }

        public string Print(int i)
        {
            var args = Arguments.Select(a => a.Print(0)).Separate(" ", prepend: " ");
            return string.Format("NEW {0}{1}", Definition.Name, args);
        }

        public override string ToString() => Print(0);
    }
}
