using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnFuncExpn : IGnFuncNode
    {
        public static IGnFuncExpn Prepare(IScope scope, IAzFuncExpn funcExpn)
        {
            return funcExpn switch
            {
                AzScopedFuncExpn scopedExpn => GnScopedFuncExpn.Prepare(scopedExpn),
                IAzLiteralExpn litExpn => IGnLiteralExpn.Prepare(scope, litExpn),
                AzSymbolExpn symExpn => GnSymbolExpn.Prepare(scope, symExpn),
                AzAppExpn appExpn => GnAppExpn.Prepare(scope, appExpn),
                AzLambdaExpn lamExpn => GnLambdaExpn.Prepare(scope, lamExpn),
                AzTupleExpn tupExpn => GnTupleExpn.Prepare(scope, tupExpn),
                AzIfExpn ifExpn => GnIfExpn.Prepare(scope, ifExpn),
                AzMatchExpn matExpn => GnMatchExpn.Prepare(scope, matExpn),
                //AzClosureExpn closeExpn => GnClosureExpn.Prepare(closeExpn),
                AzNewDataExpn newExpn => GnNewDataExpn.Prepare(scope, newExpn),
                AzGetElementExpn getExpn => GnGetElementExpn.Prepare(scope, getExpn),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
