namespace Transpiler
{
    public record AppNode(IFuncExpnNode Function,
                          IFuncExpnNode Argument) : IFuncExpnNode
    {
        public static bool Solve(Scope scope,
                                 AppNode node)
        {
            bool p = false;

            p |= IFuncExpnNode.Solve(scope, node.Function);
            p |= IFuncExpnNode.Solve(scope, node.Argument);

            var table = scope.TvTable;
            var tfx = table.GetTypeOf(node);
            var tf = table.GetTypeOf(node.Function);
            var tx = table.GetTypeOf(node.Argument);

            // If f is solved, solve (f x) and x
            if (tf.IsSolved)
            {
                if (tf is LambdaType lambdaType)
                {
                    if (!tfx.IsSolved)
                    {
                        table.SetTypeOf(node, lambdaType.Output);
                        p = true;
                    }
                    if (!tx.IsSolved)
                    {
                        table.SetTypeOf(node.Argument, lambdaType.Input);
                        p = true;
                    }
                }
                else
                {
                    throw Analyzer.Error("Expected lambda type.", node.Function);
                }
            }

            // If (f x) is solved, solve input of f
            // If x is solved, solve output of f
            if (tx.IsSolved)
            {

            }

            return p;
        }

        public string Print(int i)
        {
            return string.Format("({0} {1})", Function.Print(i), Argument.Print(i));
        }
    }
}
