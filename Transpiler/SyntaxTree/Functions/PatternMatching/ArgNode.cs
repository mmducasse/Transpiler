using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record ArgNode(string Name) : IPatternNode, IFuncDefnNode
    {
        public static bool Parse(ref TokenQueue queue, out ArgNode node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.Lowercase, ref q, out string symbol))
            {
                node = new ArgNode(symbol);
                queue = q;
                return true;
            }

            return false;
        }

        public static ArgNode Analyze(Scope scope,
                                      ArgNode node)
        {
            scope.FuncDefinitions[node.Name] = node;
            scope.TvTable.AddNode(scope, node);

            return node;
        }

        public static bool Solve(Scope scope,
                                 ArgNode node)
        {
            //var tfx = table.GetTypeOf(node);
            //var tf = table.GetTypeOf(node.Function);
            //var tx = table.GetTypeOf(node.Argument);

            return false;
        }

        public string Print(int i) => Name;
    }
}
