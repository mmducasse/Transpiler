using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnLiteralExpn : IGnFuncExpn, IGnPattern, IGnInlineNode
    {
        public static IGnLiteralExpn Prepare(IAzLiteralExpn litExpn)
        {
            return litExpn switch
            {
                AzIntLiteral intLit => GnIntLiteral.Prepare(intLit),
                AzRealLiteral realLit => GnRealLiteral.Prepare(realLit),
                AzCharLiteral charLit => GnCharLiteral.Prepare(charLit),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
