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
                                              AzTupleExpn node)
        {
            ConstraintSet cs = new();
            foreach (var e in node.Elements)
            {
                var cse = IAzFuncExpn.Constrain(tvTable, scope, e);
                cs = IConstraints.Union(cs, cse);
            }

            List<IAzTypeExpn> tes = new();
            foreach (var e in node.Elements)
            {
                var te = tvTable.GetTypeOf(e);
                tes.Add(te);
            }

            var tt = tvTable.GetTypeOf(node);

            var ct = new Constraint(tt, new AzTypeTupleExpn(tes, CodePosition.Null), node);

            return IConstraints.Union(ct, cs);
        }

        public string Print(int i)
        {
            return Elements.Select(e => e.Print(i)).Separate(", ");
        }
    }
}
