using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzAppExpn(IAzFuncExpn Function,
                            IAzFuncExpn Argument,
                            CodePosition Position) : IAzFuncExpn
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          PsArbExpn node)
        {
            var first = IAzFuncExpn.Analyze(scope, node.Children[0]);

            for (int i = 1; i < node.Children.Count; i++)
            {
                var second = IAzFuncExpn.Analyze(scope, node.Children[i]);
                first = new AzAppExpn(first, second, first.Position);
            }

            return first;
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzAppExpn node)
        {
            var csf = IAzFuncExpn.Constrain(tvTable, scope, node.Function);
            var csx = IAzFuncExpn.Constrain(tvTable, scope, node.Argument);

            var tf = tvTable.GetTypeOf(node.Function);
            var tx = tvTable.GetTypeOf(node.Argument);
            var tfx = tvTable.GetTypeOf(node);

            var p = CodePosition.Null;
            var cfx = new Constraint(tf, new AzTypeLambdaExpn(tx, tfx, p), node);

            return IConstraints.Union(cfx, csf, csx);
        }

        public string Print(int i)
        {
            return string.Format("({0} {1})", Function.Print(i), Argument.Print(i));
        }
    }
}
