
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record SymbolNode(string Name) : IFuncExpnNode, IPatternNode
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

            scope.TvTable.AddNode(scope, node);

            return node;
        }

        public static void Solve(TvTable tvTable,
                                 SymbolNode node)
        {

        }

        public string Print(int indent)
        {
            return Name;
        }
    }
}
