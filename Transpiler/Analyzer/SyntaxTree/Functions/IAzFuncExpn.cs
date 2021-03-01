using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncExpn : IAzFuncNode
    {
        IAzFuncExpn SubstituteType(Substitution s);

        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          IPsFuncExpn node)
        {
            return node switch
            {
                PsScopedFuncExpn scopedExpn => AzScopedFuncExpn.Analyze(scope, names, tvs, scopedExpn),
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, names, tvs, arbExpn),
                PsIfExpn ifExpn => AzIfExpn.Analyze(scope, names, tvs, ifExpn),
                PsLambdaExpn lamExpn => AzLambdaExpn.Analyze(scope, names, tvs, lamExpn),
                PsTupleExpn tupExpn => AzTupleExpn.Analyze(scope, names, tvs, tupExpn),
                PsSymbolExpn symExpn => AzSymbolExpn.Analyze(scope, names, tvs, symExpn),
                PsMatchExpn matExpn => AzMatchExpn.Analyze(scope, names, tvs, matExpn),
                PsClosureExpn closeExpn => AzClosureExpn.Analyze(scope, names, tvs, closeExpn),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, names, tvs, litExpn),
                _ => throw new NotImplementedException()
            };
        }
    }
}
