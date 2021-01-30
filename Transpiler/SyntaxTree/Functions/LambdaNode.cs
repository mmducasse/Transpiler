using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record LambdaNode(ArgNode Argument,
                             IFuncExpnNode Expression) : IFuncExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out LambdaNode node)
        {
            node = null;
            var q = queue;

            if (!ArgNode.Parse(ref q, out var argNode)) { return false; }
            if (!Finds("->", ref q)) { return false; }
            if (!IFuncExpnNode.Parse(ref q, out var expnNode))
            {
                throw Error("Expected expression after '->' in lambda function.", q);
            }

            node = new LambdaNode(argNode, expnNode);
            queue = q;
            return true;
        }

        public static LambdaNode Analyze(Scope scope,
                                         LambdaNode node)
        {
            var arg = ArgNode.Analyze(scope, node.Argument);
            var expr = IFuncExpnNode.Analyze(scope, node.Expression);

            var newLambdaExpr = new LambdaNode(arg, expr);
            scope.TvTable.AddNode(scope, newLambdaExpr);
            return newLambdaExpr;
        }

        public static bool Solve(Scope scope,
                                 LambdaNode node)
        {
            bool p = false;

            p |= ArgNode.Solve(scope, node.Argument);
            p |= IFuncExpnNode.Solve(scope, node.Expression);

            var table = scope.TvTable;
            var txy = table.GetTypeOf(node);
            var tx = table.GetTypeOf(node.Argument);
            var ty = table.GetTypeOf(node.Expression);

            if (txy is not LambdaType lamType)
            {
                lamType = new LambdaType(TypeVariable.Next, TypeVariable.Next);
                table.SetTypeOf(node, lamType);
            }

            if (tx.IsSolved)
            {
                if (!lamType.Input.IsSolved)
                {
                    lamType = lamType with { Input = tx };
                    table.SetTypeOf(node, lamType);
                    p = true;
                }
            }

            if (ty.IsSolved)
            {
                if (!lamType.Output.IsSolved)
                {
                    lamType = lamType with { Output = ty };
                    table.SetTypeOf(node, lamType);
                    p = true;
                }
            }

            if (txy.IsSolved)
            {
                if (!tx.IsSolved)
                {
                    table.SetTypeOf(node.Argument, lamType.Input);
                    p = true;
                }
                if (!ty.IsSolved)
                {
                    table.SetTypeOf(node.Expression, lamType.Output);
                    p = true;
                }
            }

            return p;
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Argument.Print(indent + 1),
                                 Expression.Print(indent + 1));
        }
    }
}
