using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnLiteralExpn : IGnFuncExpn, IGnPattern, IGnInlineNode
    {
        public static IGnLiteralExpn Prepare(IScope scope, IAzLiteralExpn litExpn)
        {
            return litExpn switch
            {
                AzIntLiteral intLit => GnIntLiteral.Prepare(scope, intLit),
                AzRealLiteral realLit => GnRealLiteral.Prepare(scope, realLit),
                AzCharLiteral charLit => GnCharLiteral.Prepare(scope, charLit),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
