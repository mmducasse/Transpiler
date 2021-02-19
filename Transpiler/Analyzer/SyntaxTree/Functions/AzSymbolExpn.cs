using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Analysis.Analyzer;

namespace Transpiler.Analysis
{
    // Todo: point to defn...
    public record AzSymbolExpn(IAzFuncDefn Definition,
                               CodePosition Position) : IAzFuncExpn, IAzPattern
    {
        public IAzTypeExpn Type { get; set; }

        public static AzSymbolExpn Analyze(Scope scope,
                                           PsSymbolExpn node)
        {
            if (scope.TryGetFuncDefn(node.Name, out var funcDefn))
            {
                return new AzSymbolExpn(funcDefn, node.Position);
            }

            throw Error("Undefined symbol: " + node.Name, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            // Determine the Type.
            if (Definition.ExplicitType != null)
            {
                Type = Definition.ExplicitType.WithUniqueTvs(provider);
            }
            else if (Definition.Type != null)
            {
                Type = Definition.Type;
            }
            else
            {
                Type = provider.Next;
            }

            return ConstraintSet.Empty;
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr();
        }

        public string Print(int i)
        {
            return Definition.Name;
        }

        public override string ToString() => Print(0);
    }
}
