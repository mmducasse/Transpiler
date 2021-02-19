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
        public IAzTypeExpn Type { get; set; }

        public static AzMatchCase Analyze(Scope scope,
                                          PsMatchCase node)
        {
            var pattern = IAzPattern.Analyze(scope, node.Pattern);
            var expn = IAzFuncExpn.Analyze(scope, node.Expression);

            return new(pattern, expn, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var cspatt = Pattern.Constrain(provider, scope);
            var csexpn = Expression.Constrain(provider, scope);

            Type = new AzTypeLambdaExpn(Pattern.Type, Expression.Type, Null);

            return IConstraintSet.Union(cspatt, csexpn);
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
