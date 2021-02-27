using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public record AzGetElementExpn(int ElementIndex,
                                   int NumElements,
                                   IAzFuncExpn Expression,
                                   CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            List<TypeVariable> elementTvs = new();
            for (int i = 0; i < NumElements; i++)
            {
                elementTvs.Add(provider.Next);
            }

            Type = elementTvs[ElementIndex];
            var tupType = new AzTypeTupleExpn(elementTvs, CodePosition.Null);

            var cse = Expression.Constrain(provider, scope);

            var ctup = new Constraint(tupType, Expression.Type, Position);

            return IConstraintSet.Union(cse, ctup);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Expression.GetSubnodes()).ToList();
        }

        public string Print(int i)
        {
            return string.Format("Get {0} {1}", ElementIndex, Expression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
