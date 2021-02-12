using System;
using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public class TvTable
    {
        public TvProvider TvProvider { get; } = new();

        public IReadOnlyDictionary<IAzFuncNode, IAzTypeExpn> NodeTypes => mNodeTypes;
        private Dictionary<IAzFuncNode, IAzTypeExpn> mNodeTypes = new();

        public IAzTypeExpn AddNode(Scope scope, IAzFuncNode node)
        {
            IAzTypeExpn type;

            if (node is IAzFuncDefn defn)
            {
                if (defn.ExplicitType != null)
                {
                    type = defn.ExplicitType;
                }
                else if (scope.TryGetFuncDefnType(defn, out var funcType))
                {
                    type = funcType;
                }
                else
                {
                    type = TvProvider.Next;
                }
            }
            else if (node is IAzLiteralExpn literal)
            {
                type = literal.CertainType.ToSym();
            }
            else if (node is AzSymbolExpn symbol)
            {
                if (scope.TryGetFuncDefnType(symbol.Definition, out IAzTypeExpn symType))
                {
                    type = symType.WithUniqueTvs(TvProvider);
                }
                else if (NodeTypes.TryGetValue(symbol.Definition, out IAzTypeExpn funcDefnType))
                {
                    type = funcDefnType;
                }
                else if (scope.TryGetFuncDefn(symbol.Definition.Name, out IAzFuncDefn funcDefn) &&
                         mNodeTypes.TryGetValue(funcDefn, out IAzTypeExpn typeExpn))
                {
                    type = typeExpn;
                }
                else
                {
                    type = TvProvider.Next;
                    //mNodeTypes[symbol.Definition] = type;
                }
            }
            else
            {
                type = TvProvider.Next;
            }

            mNodeTypes[node] = type;

            return type;
        }

        public IAzTypeExpn GetTypeOf(IAzFuncNode node)
        {
            return mNodeTypes[node];
        }

        public void Print(Substitution sub = null)
        {
            sub = sub ?? new Substitution();
            foreach (var (node, t) in NodeTypes)
            {
                var solvedType = IAzTypeExpn.Substitute(t, sub);
                string typeString = solvedType.Print(0);
                if (solvedType is IAzTypeDefn namedType)
                {
                    typeString = namedType.Name;
                }

                var tvs = TvUtils.GetTvs(solvedType);
                var refs = new List<(AzClassTypeDefn, TypeVariable)>();
                foreach (var tv in tvs)
                {
                    foreach (var r in tv.Refinements)
                    {
                        refs.Add((r, tv));
                    }
                }

                string refinements =
                    refs.Select(r => string.Format("{0} {1}", r.Item1.Name, r.Item2.Print())).Separate(", ");
                string IReadOnlyListow = (refinements.Count() > 0) ? " => " : " ";

                Console.Write("{0, 30} :: ", node.Print(0));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(string.Format("{0}{1}{2}", refinements, IReadOnlyListow, typeString));
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
