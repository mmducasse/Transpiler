using System;
using System.Collections.Generic;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public class AzGetElementExpn : IAzFuncExpn
    {
        public int ElementIndex { get; }
        public int NumElements { get; }
        public IAzFuncExpn Expression { get; }
        public CodePosition Position => Null;

        public IAzTypeExpn Type { get; private set; }
        private AzTypeTupleExpn TupleType { get; init; }

        public AzGetElementExpn(int elementIndex,
                                int numElements,
                                IAzFuncExpn expression)
        {
            ElementIndex = elementIndex;
            NumElements = numElements;
            Expression = expression;

            List<TypeVariable> elementTvs = new();
            for (int i = 0; i < numElements; i++)
            {
                elementTvs.Add(TypeVariables.Next);
            }

            Type = elementTvs[elementIndex];
            TupleType = new(elementTvs, Null);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var cse = Expression.Constrain(provider, scope);

            var ctup = new Constraint(TupleType, Expression.Type, Position);

            return IConstraintSet.Union(cse, ctup);
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Expression.Recurse(action);
            action(this);
        }

        public string Print(int i)
        {
            return string.Format("Get {0} {1}", ElementIndex, Expression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
