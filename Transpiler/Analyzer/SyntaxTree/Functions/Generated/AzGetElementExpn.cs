using System.Collections.Generic;
using System.Linq;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzGetElementExpn(int ElementIndex,
                                   int NumElements,
                                   IAzFuncExpn Expression,
                                   IAzTypeExpn Type,
                                   CodePosition Position) : IAzFuncExpn
    {
        private AzTypeTupleExpn TupleType { get; init; }

        public static AzGetElementExpn Make(int elementIndex,
                                            int numElements,
                                            IAzFuncExpn expression,
                                            TvProvider tvs)
        {
            List<TypeVariable> elementTvs = new();
            for (int i = 0; i < numElements; i++)
            {
                elementTvs.Add(tvs.Next);
            }

            var type = elementTvs[elementIndex];
            return new(elementIndex, numElements, expression, type, Null)
            {
                TupleType = new(elementTvs, Null)
            };
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var cse = Expression.Constrain(provider, scope);

            var ctup = new Constraint(TupleType, Expression.Type, Position);

            return IConstraintSet.Union(cse, ctup);
        }

        public IAzFuncExpn SubstituteType(Substitution s)
        {
            return this with
            {
                Expression = Expression.SubstituteType(s),
                Type = Type.Substitute(s),
            };
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
