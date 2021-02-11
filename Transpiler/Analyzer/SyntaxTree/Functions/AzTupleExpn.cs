using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTupleExpn(IReadOnlyList<IAzFuncExpn> Elements,
                              CodePosition Position) : IAzFuncExpn
    {
        public static AzTupleExpn Analyze(Scope scope,
                                          PsTupleExpn node)
        {
            List<IAzFuncExpn> elements = new();
            foreach (var n in node.Elements)
            {
                var expn = IAzFuncExpn.Analyze(scope, n);
                elements.Add(expn);
            }

            return new(elements, node.Position);
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

            var cf = new Constraint(tf, new AzTypeLambdaExpn(tx, te, CodePosition.Null), node);

            return IConstraints.Union(cf, cse);
        }

        public string Print(int i)
        {
            return Elements.Select(e => e.Print(i)).Separate(", ");
        }
    }
}
