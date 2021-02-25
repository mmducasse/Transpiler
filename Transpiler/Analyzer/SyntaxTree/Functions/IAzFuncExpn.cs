using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncExpn : IAzFuncStmt
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider provider,
                                          IPsFuncExpn node)
        {
            return node switch
            {
                PsScopedFuncExpn scopedExpn => AzScopedFuncExpn.Analyze(scope, provider, scopedExpn),
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, provider, arbExpn),
                PsIfExpn ifExpn => AzIfExpn.Analyze(scope, provider, ifExpn),
                PsLambdaExpn lamExpn => AzLambdaExpn.Analyze(scope, provider, lamExpn),
                PsTupleExpn tupExpn => AzTupleExpn.Analyze(scope, provider, tupExpn),
                PsSymbolExpn symExpn => AzSymbolExpn.Analyze(scope, provider, symExpn),
                PsMatchExpn matExpn => AzMatchExpn.Analyze(scope, provider, matExpn),
                PsClosureExpn closeExpn => AzClosureExpn.Analyze(scope, provider, closeExpn),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, provider, litExpn),
                _ => throw new NotImplementedException()
            };
        }
    }
}
