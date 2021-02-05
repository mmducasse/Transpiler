using System;
using System.Collections.Generic;

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
                    type = symType;
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

                Console.Write("{0, 30} :: ", node.Print(0));
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(typeString);
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
    }
}
