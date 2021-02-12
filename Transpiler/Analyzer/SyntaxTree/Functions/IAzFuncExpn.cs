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
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, arbExpn),
                PsIfExpn ifExpn => AzIfExpn.Analyze(scope, ifExpn),
                PsLambdaExpn lamExpn => AzLambdaExpn.Analyze(scope, lamExpn),
                PsTupleExpn tupExpn => AzTupleExpn.Analyze(scope, tupExpn),
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
                AzGenDataExpn genExpn => ConstraintSet.Empty,
                AzSymbolExpn symExpn => AzSymbolExpn.Constrain(tvTable, scope, symExpn),
                AzAppExpn appExpn => AzAppExpn.Constrain(tvTable, scope, appExpn),
                AzLambdaExpn lamExpn => AzLambdaExpn.Constrain(tvTable, scope, lamExpn),
                AzIfExpn ifExpn => AzIfExpn.Constrain(tvTable, scope, ifExpn),
                AzTupleExpn tupExpn => AzTupleExpn.Constrain(tvTable, scope, tupExpn),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
