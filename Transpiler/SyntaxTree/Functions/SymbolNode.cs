using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record SymbolNode(string Name,
                             bool ForcePrefix = false) : IFuncExpnNode, IPatternNode
    {
        public static bool Parse(ref TokenQueue queue, out SymbolNode node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.Alphabetic | TokenType.Symbol, ref q, out string symbol))
            {
                node = new SymbolNode(symbol);
                queue = q;
                return true;
            }

            return false;
        }

        public static SymbolNode Analyze(Scope scope,
                                         SymbolNode node)
        {
            if (!scope.VerifySymbols(node.Name))
            {
                throw Analyzer.Error("Undefined symbol: " + node.Name, node);
            }

            return node;
        }

        public static void Solve(TvTable tvTable,
                                 SymbolNode node)
        {

        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              SymbolNode node)
        {
            //var tvTable = scope.TvTable;

            //var ts = tvTable.GetTypeOf(node);
            //var td = tvTable.GetTypeOf(scope.Definitions[node.Name]);

            //var cf = new Constraint(ts, td);

            return IConstraints.Union();
        }

        public string Print(int indent)
        {
            return Name;
        }
    }
}
