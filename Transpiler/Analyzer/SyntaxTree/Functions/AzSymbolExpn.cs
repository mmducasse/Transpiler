using Transpiler.Parse;
using static Transpiler.Analysis.Analyzer;

namespace Transpiler.Analysis
{
    // Todo: point to defn...
    public record AzSymbolExpn(IAzFuncDefn Definition,
                               CodePosition Position) : IAzFuncExpn, IAzPattern
    {
        public static AzSymbolExpn Analyze(Scope scope,
                                           PsSymbolExpn node)
        {
            if (scope.TryGetFuncDefn(node.Name, out var funcDefn))
            {
                return new AzSymbolExpn(funcDefn, node.Position);
            }

            throw Error("Undefined symbol: " + node.Name, node.Position);
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzSymbolExpn node)
        {
            //var tvTable = scope.TvTable;

            //var ts = tvTable.GetTypeOf(node);
            //var td = tvTable.GetTypeOf(scope.Definitions[node.Name]);

            //var cf = new Constraint(ts, td);

            return IConstraintSet.Union();
        }

        public string Print(int i)
        {
            return Definition.Name;
        }

        public override string ToString() => Print(0);
    }
}
