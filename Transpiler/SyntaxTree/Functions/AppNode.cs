using System.Collections.Generic;

namespace Transpiler
{
    public record AppNode(IFuncExpnNode Function,
                          IFuncExpnNode Argument) : IFuncExpnNode
    {
        public static ConstraintSet Constrain(Scope scope,
                                                 AppNode node)
        {
            var tvTable = scope.TvTable;

            var tf = tvTable.GetTypeOf(node.Function);
            var tx = tvTable.GetTypeOf(node.Argument);
            var tfx = tvTable.GetTypeOf(node);

            var cfx = new Constraint(tf, new FunType(tx, tfx), node);

            var csf = IFuncExpnNode.Constrain(scope, node.Function);
            var csx = IFuncExpnNode.Constrain(scope, node.Argument);

            return IConstraints.Union(cfx, csf, csx);
        }

        public string Print(int i)
        {
            return string.Format("({0} {1})", Function.Print(i), Argument.Print(i));
        }
    }
}
