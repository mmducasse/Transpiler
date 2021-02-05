using System.Collections.Generic;

namespace Transpiler
{
    public record AppNode(IFuncExpnNode Function,
                          IFuncExpnNode Argument) : IFuncExpnNode
    {
        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AppNode node)
        {
            var csf = IFuncExpnNode.Constrain(tvTable, scope, node.Function);
            var csx = IFuncExpnNode.Constrain(tvTable, scope, node.Argument);

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
