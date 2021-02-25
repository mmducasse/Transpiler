using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncExpn : IAzFuncStmt
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          IPsFuncExpn node)
        {
            return node switch
            {
                PsScopedFuncExpn scopedExpn => AzScopedFuncExpn.Analyze(scope, scopedExpn),
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, arbExpn),
                PsIfExpn ifExpn => AzIfExpn.Analyze(scope, ifExpn),
                PsLambdaExpn lamExpn => AzLambdaExpn.Analyze(scope, lamExpn),
                PsTupleExpn tupExpn => AzTupleExpn.Analyze(scope, tupExpn),
                PsSymbolExpn symExpn => AzSymbolExpn.Analyze(scope, symExpn),
                PsMatchExpn matExpn => AzMatchExpn.Analyze(scope, matExpn),
                PsClosureExpn closeExpn => AzClosureExpn.Analyze(scope, closeExpn),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, litExpn),
                _ => throw new NotImplementedException()
            };
        }
    }
}
