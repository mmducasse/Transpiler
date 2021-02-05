using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record ParamNode(string Name) : IPatternNode, IFuncDefnNode
    {
        public eFixity Fixity => eFixity.Prefix;

        public static bool Parse(ref TokenQueue queue, out ParamNode node)
        {
            node = null;
            var q = queue;

            if (Finds(TokenType.Lowercase, ref q, out string symbol))
            {
                node = new ParamNode(symbol);
                queue = q;
                return true;
            }

            return false;
        }

        public static ParamNode Analyze(Scope scope,
                                      ParamNode node)
        {
            scope.FuncDefinitions[node.Name] = node;

            return node;
        }

        public static bool Solve(Scope scope,
                                 ParamNode node)
        {
            //var tfx = table.GetTypeOf(node);
            //var tf = table.GetTypeOf(node.Function);
            //var tx = table.GetTypeOf(node.Argument);

            return false;
        }

        public string Print(int i) => Name;
    }
}
