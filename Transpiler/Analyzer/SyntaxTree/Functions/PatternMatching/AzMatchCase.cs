using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzMatchCase(IAzPattern Pattern,
                              IAzFuncExpn Expression,
                              CodePosition Position) : IAzFuncNode
    {
        public IAzTypeExpn Type { get; private set; }

        public static AzMatchCase Analyze(Scope scope,
                                          NameProvider names,
                                          PsMatchCase node)
        {
            var pattern = IAzPattern.Analyze(scope, names, node.Pattern);
            var expn = IAzFuncExpn.Analyze(scope, names, node.Expression);

            AzTypeLambdaExpn type = new(TypeVariables.Next, TypeVariables.Next, Null);
            return new(pattern, expn, node.Position) { Type = type };
        }

        public ConstraintSet Constrain(TvProvider tvs, Scope scope)
        {
            var cspatt = Pattern.Constrain(tvs, scope);
            var csexpn = Expression.Constrain(tvs, scope);

            var lamType = Type as AzTypeLambdaExpn;
            var cinput = new Constraint(lamType.Input, Pattern.Type, lamType.Input.Position);
            var coutput = new Constraint(lamType.Output, Expression.Type, lamType.Output.Position);

            return IConstraintSet.Union(cinput, coutput, cspatt, csexpn);
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Pattern.Recurse(action);
            Expression.Recurse(action);
            action(this);
        }

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), Expression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
