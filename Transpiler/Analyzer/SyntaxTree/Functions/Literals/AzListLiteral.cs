using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class AzListLiteral
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          PsListLiteral listLiteral)
        {
            scope.TryGetFuncDefn("Empty", out var emptyCtor);
            IAzFuncExpn empty = new AzSymbolExpn(emptyCtor, CodePosition.Null);

            scope.TryGetFuncDefn("Node", out var nodeCtor);
            IAzFuncExpn node = new AzSymbolExpn(nodeCtor, CodePosition.Null);

            IAzFuncExpn head = empty;

            foreach (var e in listLiteral.Elements.Reverse())
            {
                var p = e.Position;
                var element = IAzFuncExpn.Analyze(scope, e);
                head = new AzAppExpn(new AzAppExpn(node, element, p), head, p);
            }

            return head;
        }
    }
}
