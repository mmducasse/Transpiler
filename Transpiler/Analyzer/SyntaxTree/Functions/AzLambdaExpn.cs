using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzLambdaExpn(AzParam Parameter,
                               IAzFuncExpn Expression,
                               CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public static AzLambdaExpn Analyze(Scope scope,
                                           NameProvider provider,
                                           PsLambdaExpn node)
        {
            var arg = AzParam.Analyze(scope, provider, node.Parameter);
            var expr = IAzFuncExpn.Analyze(scope, provider, node.Expression);

            var newLambdaExpr = new AzLambdaExpn(arg, expr, node.Position);
            return newLambdaExpr;
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            // Initialize Type to a new Type Variable.
            Type = provider.Next;

            var csp = Parameter.Constrain(provider, scope);
            var cse = Expression.Constrain(provider, scope);

            var lamType = new AzTypeLambdaExpn(Parameter.Type, Expression.Type, Null);
            var cf = new Constraint(Type, lamType, Position);

            return IConstraintSet.Union(cf, csp, cse);
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
