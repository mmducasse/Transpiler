using System;
using System.Collections.Generic;

namespace Transpiler
{
    public record MutableTv(IAstNode Node, IType Type);

    public class TvTable
    {
        private Dictionary<IAstNode, int> mNodeTypeIndices = new();

        public IReadOnlyList<MutableTv> Types => mTypes;
        private List<MutableTv> mTypes = new();

        public IType AddNode(Scope scope, IAstNode node)
        {
            IType type;
            int typeIndex = mTypes.Count;
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
                    var defnNode = scope.Definitions[symbol.Name];
                    type = GetTypeOf(defnNode);
                }
            }
            else
            {
                type = TypeVariable.Next;
            }

            mNodeTypeIndices[node] = typeIndex;
            mTypes.Add(new(node, type));

            return type;
        }

        public IType GetTypeOf(IAstNode node)
        {
            int index = mNodeTypeIndices[node];
            return mTypes[index].Type;
        }

        public void SetTypeOf(IAstNode node, IType newType)
        {
            var oldType = GetTypeOf(node);
            IType type = null;
            try
            {
                type = IType.Unify(oldType, newType);
            }
            catch (InvalidOperationException e)
            {
                string msg = string.Format("Unable to unify types {0} and {1}.", oldType.Print(), newType.Print());
                throw Analyzer.Error(msg, node);
            }
            Substitute(oldType, type);

            //int index = mNodeTypeIndices[node];
            //mTypes[index] = mTypes[index] with { Type = type };
        }

        public bool EnsureEqual(IAstNode node1, IAstNode node2)
        {
            var t1 = GetTypeOf(node1);
            var t2 = GetTypeOf(node2);

            if (t1 == t2) { return false; }

            bool p = false;

            IType t = null;
            try
            {
                t = IType.Unify(t1, t2);
            }
            catch
            {
                string msg = string.Format("Unable to force type equivalence between {0} and {1}.", t1, t2);
                throw Analyzer.Error(msg, node1);
            }

            if (t1 != t)
            {
                SetTypeOf(node1, t);
                p = true;
            }
            if (t2 != t)
            {
                SetTypeOf(node2, t);
                p = true;
            }

            return p;
        }

        public void Substitute(IType orignal,
                               IType substitute)
        {
            var sub = new Substitution(orignal, substitute);
            for (int i = 0; i < mTypes.Count; i++)
            {
                var newType = IType.Substitute(mTypes[i].Type, sub);
                mTypes[i] = mTypes[i] with { Type = newType };
                //if (mTypes[i].Type == orignal)
                //{
                //    // Todo: Implement IType.SUbstitute.
                //    mTypes[i] = mTypes[i] with { Type = substitute };
                //}
            }
        }

        public void Print()
        {
            Console.WriteLine("\n === TV TABLE ===");
            foreach (IAstNode node in mNodeTypeIndices.Keys)
            {
                var tv = mTypes[mNodeTypeIndices[node]];
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("{0} :: {1}", node.Abbr(), tv.Type.Print());
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("{0}\n", node.Print(0));
            }
        }

        //public IType GetTypeFor(string symbol)
        //{
        //    int tvId = mNodeTypeIndices[symbol];
        //    return mTvValues[tvId];
        //}
    }
}
