using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnFuncExpn : IGnFuncNode
    {
        public static IGnFuncExpn Prepare(IAzFuncExpn funcExpn)
        {
            return funcExpn switch
            {
                AzScopedFuncExpn scopedExpn => GnScopedFuncExpn.Prepare(scopedExpn),
                IAzLiteralExpn litExpn => IGnLiteralExpn.Prepare(litExpn),
                AzSymbolExpn symExpn => GnSymbolExpn.Prepare(symExpn),
                AzAppExpn appExpn => GnAppExpn.Prepare(appExpn),
                AzLambdaExpn lamExpn => GnLambdaExpn.Prepare(lamExpn),
                AzIfExpn ifExpn => GnIfExpn.Prepare(ifExpn),
                AzMatchExpn matExpn => GnMatchExpn.Prepare(matExpn),
                AzNewDataExpn newExpn => GnNewDataExpn.Prepare(newExpn),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
