using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class AzListLiteral
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          PsListLiteral listLiteral)
        {
            var elements = listLiteral.Elements.Select(e => IAzFuncExpn.Analyze(scope, names, tvs, e)).ToList();

            return CreateList(scope, tvs, elements);
        }

        public static IAzFuncExpn CreateList(Scope scope, TvProvider tvs, IReadOnlyList<IAzFuncExpn> elements)
        {
            scope.TryGetFuncDefn("Empty", out var emptyCtor);
            var emptyType = emptyCtor.Type.WithUniqueTvs(tvs);
            IAzFuncExpn empty = new AzSymbolExpn(emptyCtor, emptyType, CodePosition.Null);

            scope.TryGetFuncDefn("Node", out var nodeCtor);
            var nodeType = nodeCtor.Type.WithUniqueTvs(tvs);
            IAzFuncExpn node = new AzSymbolExpn(nodeCtor, nodeType, CodePosition.Null);

            IAzFuncExpn head = empty;

            foreach (var e in elements.Reverse())
            {
                var p = e.Position;
                head = new AzAppExpn(new AzAppExpn(node, e, tvs.Next, p), head, tvs.Next, p);
            }

            return head;
        }
    }
}
