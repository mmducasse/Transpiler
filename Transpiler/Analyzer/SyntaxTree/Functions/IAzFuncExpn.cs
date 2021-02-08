using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncExpn : IAzFuncNode
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          IPsFuncExpn node)
        {
            return node switch
            {
                PsIfExpn ifExpn => AzIfExpn.Analyze(scope, ifExpn),
                PsLambdaExpn lamExpn => AzLambdaExpn.Analyze(scope, lamExpn),
                PsSymbolExpn symExpn => AzSymbolExpn.Analyze(scope, symExpn),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, litExpn),
                _ => throw new NotImplementedException()
            };
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              IAzFuncExpn node)
        {
            tvTable.AddNode(scope, node);

            return node switch
            {
                IAzLiteralExpn _ => ConstraintSet.Empty,
                AzSymbolExpn symExpn => AzSymbolExpn.Constrain(tvTable, scope, symExpn),
                AzAppExpn appExpn => AzAppExpn.Constrain(tvTable, scope, appExpn),
                AzLambdaExpn lamExpn => AzLambdaExpn.Constrain(tvTable, scope, lamExpn),
                AzIfExpn ifExpn => AzIfExpn.Constrain(tvTable, scope, ifExpn),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
