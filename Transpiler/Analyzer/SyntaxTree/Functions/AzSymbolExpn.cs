using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Analysis.Analyzer;

namespace Transpiler.Analysis
{
    public record AzSymbolExpn(IAzFuncDefn Definition,
                               IAzTypeExpn Type,
                               CodePosition Position) : IAzFuncExpn, IAzPattern
    {
        public static AzSymbolExpn Analyze(Scope scope,
                                           NameProvider _,
                                           TvProvider tvs,
                                           PsSymbolExpn node)
        {
            if (scope.TryGetFuncDefn(node.Name, out var funcDefn))
            {
                IAzTypeExpn type;
                // Determine the Type.
                if (funcDefn.IsSolved)
                {
                    type = funcDefn.Type.WithUniqueTvs(tvs);
                }
                else if (funcDefn.Type != null)
                {
                    type = funcDefn.Type;
                }
                else if (funcDefn is IAzFuncStmtDefn stmtDefn)
                {
                    type = tvs.Next;
                    stmtDefn.Type = type;
                    //throw Analyzer.Error("Function " + funcDefn.Name + " is not defined.", funcDefn.Position);
                }
                else
                {
                    throw new System.Exception();
                }

                return new AzSymbolExpn(funcDefn, type, node.Position);
            }

            throw Error("Undefined symbol: " + node.Name, node.Position);
        }

        public ConstraintSet Constrain(TvProvider tvs, Scope scope) => ConstraintSet.Empty;

        public IAzFuncExpn SubstituteType(Substitution s) =>
            this with { Type = Type.Substitute(s) };

        IAzPattern IAzPattern.SubstituteType(Substitution s) =>
            this with { Type = Type.Substitute(s) };

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr();
        }

        public string Print(int i) => Definition.Name;

        public override string ToString() => Print(0);
    }
}
