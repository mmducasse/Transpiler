using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzLambdaExpn(AzParam Parameter,
                               IAzFuncExpn Expression,
                               CodePosition Position) : IAzFuncExpn
    {
        public static AzLambdaExpn Analyze(Scope scope,
                                           PsLambdaExpn node)
        {
            var arg = AzParam.Analyze(scope, node.Parameter);
            var expr = IAzFuncExpn.Analyze(scope, node.Expression);

            var newLambdaExpr = new AzLambdaExpn(arg, expr, node.Position);
            return newLambdaExpr;
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzLambdaExpn node)
        {
            tvTable.AddNode(scope, node.Parameter);
            var cse = IAzFuncExpn.Constrain(tvTable, scope, node.Expression);

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
