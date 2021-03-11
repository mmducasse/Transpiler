using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncExpn : IAzFuncNode
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider names,
                                          IPsFuncExpn node)
        {
            return node switch
            {
                PsScopedFuncExpn scopedExpn => AzScopedFuncExpn.Analyze(scope, names, scopedExpn),
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, names, arbExpn),
                PsIfExpn ifExpn => AzIfExpn.Analyze(scope, names, ifExpn),
                PsLambdaExpn lamExpn => AzLambdaExpn.Analyze(scope, names, lamExpn),
                PsTupleExpn tupExpn => AzTupleExpn.Analyze(scope, names, tupExpn),
                PsSymbolExpn symExpn => AzSymbolExpn.Analyze(scope, names, symExpn),
                PsMatchExpn matExpn => AzMatchExpn.Analyze(scope, names, matExpn),
                PsClosureExpn closeExpn => AzClosureExpn.Analyze(scope, names, closeExpn),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, names, litExpn),
                _ => throw new NotImplementedException()
            };
        }
    }
}
