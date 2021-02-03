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
                if (scope.TryGetTypeForDefnName(symbol.Name, out IType symType))
                {
                    type = symType;
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

        public void Print()
        {
            Console.WriteLine("\n === TV TABLE ===");
            foreach (IAstNode node in mNodeTypes.Keys)
            {
                var tv = mNodeTypes[node];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0} :: {1}", node.Abbr(), tv.Print());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0}\n", node.Print(0));
            }
        }
    }
}
