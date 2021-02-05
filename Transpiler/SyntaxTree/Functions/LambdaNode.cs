using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record LambdaNode(ParamNode Parameter,
                             IFuncExpnNode Expression) : IFuncExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out LambdaNode node)
        {
            node = null;
            var q = queue;

            if (!ParamNode.Parse(ref q, out var argNode)) { return false; }
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
            var arg = ParamNode.Analyze(scope, node.Parameter);
            var expr = IFuncExpnNode.Analyze(scope, node.Expression);

            var newLambdaExpr = new LambdaNode(arg, expr);
            return newLambdaExpr;
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              LambdaNode node)
        {
            tvTable.AddNode(scope, node.Parameter);
            var cse = IFuncExpnNode.Constrain(tvTable, scope, node.Expression);

            var te = tvTable.GetTypeOf(node.Expression);
            var tx = tvTable.GetTypeOf(node.Parameter);
            var tf = tvTable.GetTypeOf(node);

            var cf = new Constraint(tf, new FunType(tx, te), node);

            return IConstraints.Union(cf, cse);
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Parameter.Print(indent + 1),
                                 Expression.Print(indent + 1));
        }
    }
}
