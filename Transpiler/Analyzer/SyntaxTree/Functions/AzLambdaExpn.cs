using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzLambdaExpn(AzParam Parameter,
                               IAzFuncExpn Expression,
                               IAzTypeExpn Type,
                               CodePosition Position) : IAzFuncExpn
    {
        public static AzLambdaExpn Analyze(Scope scope,
                                           NameProvider names,
                                           TvProvider tvs,
                                           PsLambdaExpn psLamExpn)
        {
            var arg = AzParam.Analyze(scope, names, tvs, psLamExpn.Parameter);
            var expr = IAzFuncExpn.Analyze(scope, names, tvs, psLamExpn.Expression);

            return new(arg, expr, tvs.Next, psLamExpn.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var csp = Parameter.Constrain(provider, scope);
            var cse = Expression.Constrain(provider, scope);

            var lamType = new AzTypeLambdaExpn(Parameter.Type, Expression.Type, Null);
            var cf = new Constraint(Type, lamType, Position);

            return IConstraintSet.Union(cf, csp, cse);
        }


        IAzFuncExpn IAzFuncExpn.SubstituteType(Substitution s) => SubstituteType(s);
        public AzLambdaExpn SubstituteType(Substitution s)
        {
            return new AzLambdaExpn(Parameter.SubstituteType(s),
                                    Expression.SubstituteType(s),
                                    Type.Substitute(s),
                                    Position);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Parameter.GetSubnodes()).Concat(Expression.GetSubnodes()).ToList();
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Parameter.Print(indent + 1),
                                 Expression.Print(indent + 1));
        }
    }
}
