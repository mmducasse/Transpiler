using System;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Analysis
{
    public record AzMatchCase(IAzPattern Pattern,
                              AzScopedFuncExpn ScopedExpression,
                              CodePosition Position) : IAzFuncNode
    {

        public static AzMatchCase Analyze(Scope scope,
                                          PsMatchCase node)
        {
            var pattern = IAzPattern.Analyze(scope, node.Pattern);
            var expn = AzScopedFuncExpn.Analyze(scope, node.Expression);

            return new(pattern, expn, node.Position);
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzMatchCase node)
        {
            tvTable.AddNode(scope, node.Pattern);

            var cspatt = IAzPattern.Constrain(tvTable, scope, node.Pattern);
            var csexpn = AzScopedFuncExpn.Constrain(tvTable, node.ScopedExpression);

            var tcase = tvTable.GetTypeOf(node);
            var tpatt = tvTable.GetTypeOf(node.Pattern);
            var texpn = tvTable.GetTypeOf(node.ScopedExpression.Expression);

            var cs = new Constraint(tcase, new AzTypeLambdaExpn(tpatt, texpn, CodePosition.Null), node);

            return IConstraintSet.Union(cs, cspatt, csexpn);
        }

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), ScopedExpression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
