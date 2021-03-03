using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class AzListLiteral
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider names,
                                          PsListLiteral listLiteral)
        {
            var elements = listLiteral.Elements.Select(e => IAzFuncExpn.Analyze(scope, names, e)).ToList();

            return CreateList(scope, elements);
        }

        public static IAzFuncExpn CreateList(Scope scope, IReadOnlyList<IAzFuncExpn> elements)
        {
            scope.TryGetFuncDefn("Empty", out var emptyCtor);
            var emptyType = emptyCtor.Type.WithUniqueTvs(TypeVariables.Provider);
            IAzFuncExpn empty = new AzSymbolExpn(emptyCtor, emptyType, CodePosition.Null);

            scope.TryGetFuncDefn("Node", out var nodeCtor);
            var nodeType = nodeCtor.Type.WithUniqueTvs(TypeVariables.Provider);
            IAzFuncExpn node = new AzSymbolExpn(nodeCtor, nodeType, CodePosition.Null);

            IAzFuncExpn head = empty;

            foreach (var e in elements.Reverse())
            {
                var p = e.Position;
                head = new AzAppExpn(new AzAppExpn(node, e, p), head, p);
            }

            return head;
        }
    }
}
