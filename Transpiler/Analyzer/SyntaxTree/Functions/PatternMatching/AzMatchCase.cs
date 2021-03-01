using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzMatchCase(IAzPattern Pattern,
                              IAzFuncExpn Expression,
                              IAzTypeExpn Type,
                              CodePosition Position) : IAzFuncNode
    {
        public static AzMatchCase Analyze(Scope scope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          PsMatchCase node)
        {
            var pattern = IAzPattern.Analyze(scope, names, tvs, node.Pattern);
            var expn = IAzFuncExpn.Analyze(scope, names, tvs, node.Expression);

            var type = new AzTypeLambdaExpn(tvs.Next, tvs.Next, Null);
            return new(pattern, expn, type, node.Position);
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

        public AzMatchCase SubstituteType(Substitution s)
        {
            return new(Pattern.SubstituteType(s),
                       Expression.SubstituteType(s),
                       Type.Substitute(s),
                       Position);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Pattern.GetSubnodes()).Concat(Expression.GetSubnodes()).ToList();
        }

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), Expression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
