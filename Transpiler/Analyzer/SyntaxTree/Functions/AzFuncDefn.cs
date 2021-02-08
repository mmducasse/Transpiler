namespace Transpiler.Analysis
{
    public interface IAzFuncDefn : IAzDefn
    {
        string Name { get; }

        //eFixity Fixity { get; }
    }

    // Todo: Add optional Type constraint property.
    public record AzFuncDefn(string Name,
                             AzScopedFuncExpn ScopedExpression,
                             CodePosition Position) : IAzFuncDefn
    {
        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzFuncDefn node)
        {
            tvTable.AddNode(scope, node);

            var cs = AzScopedFuncExpn.Constrain(tvTable, node.ScopedExpression);

            var tf = tvTable.GetTypeOf(node);
            var te = tvTable.GetTypeOf(node.ScopedExpression.Expression);

            var c = new Constraint(tf, te, node);

            return IConstraints.Union(c, cs);
        }

        public string Print(int indent)
        {
            return string.Format("{0} = {1}", Name, ScopedExpression.Print(indent + 1));
        }
    }
}
