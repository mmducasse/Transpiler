using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record ArbitraryExpnNode(IReadOnlyList<IFuncExpnNode> Children) : IFuncExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out ArbitraryExpnNode node)
        {
            node = null;
            var q = queue;
            bool doContinue = true;

            List<IFuncExpnNode> subExpns = new();
            while (doContinue)
            {
                var q2 = q;
                if (Finds("(", ref q2))
                {
                    if (!IFuncExpnNode.ParseInline(ref q2, out var arbSubNode))
                    {
                        throw Error("Expected inline expression after '('", q2);
                    }

                    subExpns.Add(arbSubNode);
                    Expects(")", ref q2);
                }
                else if (Finds(")", ref q2))
                {
                    doContinue = false;
                    break;
                }
                else if (ParseSimple(ref q2, out var simpleNode))
                {
                    subExpns.Add(simpleNode);
                }
                //else if (IFuncExpnNode.ParseMultiline(ref q2, out var expnNode))
                //{
                //    subExpns.Add(expnNode);
                //    doContinue = false;
                //}
                else
                {
                    doContinue = false;
                }

                q = q2;
            }

            if (subExpns.Count > 0)
            {
                node = new ArbitraryExpnNode(subExpns);
                queue = q;
                return true;
            }

            return false;
        }

        public static bool ParseSimple(ref TokenQueue queue, out IFuncExpnNode node)
        {
            node = null;
            var q = queue;

            if (ILiteralNode.Parse(ref q, out var litNode)) { node = litNode; }
            else if (SymbolNode.Parse(ref q, out var varNode)) { node = varNode; }
            // Operator?
            // Parens?

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }

        public static IFuncExpnNode Analyze(Scope scope,
                                            ArbitraryExpnNode node)
        {
            switch (node.Children.Count)
            {
                case 0:
                    throw Analyzer.Error("Arbitrary expression contains no sub expressions", node);
                case 1:
                    var expr = IFuncExpnNode.Analyze(scope, node.Children[0]);
                    if (expr is SymbolNode symNode &&
                        scope.TryGetFuncDefn(symNode.Name, out var funcDefn) &&
                        funcDefn.Fixity == eFixity.Infix)
                    {
                        return symNode with { ForcePrefix = true };
                    }
                    return expr;
                default:
                    // Todo: Handle infix operators...
                    var first = IFuncExpnNode.Analyze(scope, node.Children[0]);
                    var second = IFuncExpnNode.Analyze(scope, node.Children[1]);
                    bool infix = false;
                    if (second is SymbolNode symNode2 &&
                        !symNode2.ForcePrefix &&
                        scope.TryGetFuncDefn(symNode2.Name, out var funcDefn2) &&
                        funcDefn2.Fixity == eFixity.Infix)
                    {
                        infix = true;
                    }

                    var appExpr = infix ? new AppNode(second, first) : new AppNode(first, second);
                    for (int i = 2; i < node.Children.Count; i++)
                    {
                        second = IFuncExpnNode.Analyze(scope, node.Children[i]);
                        appExpr = new(appExpr, second);
                    }

                    return appExpr;
            }


            
        }

        public string Print(int i)
        {
            var children = Children.Select(c => c.Print(i)).Separate(" ");
            return string.Format("({0})", children);
        }
    }
}
