namespace Transpiler.Analysis
{
    public record AzAppExpn(IAzFuncExpn Function,
                            IAzFuncExpn Argument,
                            CodePosition Position) : IAzFuncExpn
    {
        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzAppExpn node)
        {
            var csf = IAzFuncExpn.Constrain(tvTable, scope, node.Function);
            var csx = IAzFuncExpn.Constrain(tvTable, scope, node.Argument);

            var tf = tvTable.GetTypeOf(node.Function);
            var tx = tvTable.GetTypeOf(node.Argument);
            var tfx = tvTable.GetTypeOf(node);

            var cfx = new Constraint(tf, new FunType(tx, tfx), node);

            return IConstraints.Union(cfx, csf, csx);
        }

        public string Print(int i)
        {
            return string.Format("({0} {1})", Function.Print(i), Argument.Print(i));
        }
    }
}
