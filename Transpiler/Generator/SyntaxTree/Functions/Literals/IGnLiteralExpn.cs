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
                _ => throw new NotImplementedException(),
            };
        }
    }
}
