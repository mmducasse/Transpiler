using System;
using System.Collections.Generic;
using System.Linq;

namespace Transpiler
{
    public class TvTable
    {
        public IReadOnlyDictionary<IAstNode, IType> NodeTypes => mNodeTypes;
        private Dictionary<IAstNode, IType> mNodeTypes = new();

        public IType AddNode(Scope scope, IAstNode node)
        {
            IType type;

            if (node is ILiteralNode literal)
            {
                type = literal.CertainType;
            }
            else if (node is SymbolNode symbol)
            {
                if (scope.TryGetFuncDefnType(symbol.Name, out IType symType))
                {
                    type = TvUtils.MadeUnique(symType);
                }
                else if (scope.TryGetFuncDefn(symbol.Name, out IFuncDefnNode funcDefn) &&
                         NodeTypes.TryGetValue(funcDefn, out IType funcDefnType))
                {
                    type = funcDefnType;
                }
                else
                {
                    var defnNode = scope.FuncDefinitions[symbol.Name];
                    type = GetTypeOf(defnNode);
                }
            }
            else
            {
                type = TypeVariable.Next;
            }

            mNodeTypes[node] = type;

            return type;
        }

        public IType GetTypeOf(IAstNode node)
        {
            return mNodeTypes[node];
        }

        public void Print(Substitution sub = null)
        {
            sub = sub ?? new Substitution();
            foreach (var (node, t) in NodeTypes)
            {
                var solvedType = IType.Substitute(t, sub);
                string typeString = solvedType.Print();
                if (solvedType is INamedType namedType)
                {
                    typeString = namedType.Name;
                }

                var tvs = TvUtils.GetTvs(solvedType);
                var refs = new List<(IClassType, TypeVariable)>();
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
