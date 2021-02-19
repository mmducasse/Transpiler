﻿using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzParam(string Name,
                          CodePosition Position) : IAzPattern, IAzFuncDefn
    {
        public IAzTypeExpn ExplicitType => null;

        public eFixity Fixity => eFixity.Prefix;

        public IAzTypeExpn Type { get; set; }

        public static AzParam Analyze(Scope scope,
                                      PsParam node)
        {
            var azParam = new AzParam(node.Name, node.Position);

            scope.FuncDefinitions[azParam.Name] = azParam;

            return azParam;
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            return ConstraintSet.Empty;
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr();
        }

        public string Print(int i) => Name;

        public override string ToString() => Print(0);
    }
}
