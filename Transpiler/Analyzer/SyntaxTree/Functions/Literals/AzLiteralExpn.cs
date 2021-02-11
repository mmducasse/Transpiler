using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzLiteralExpn : IAzFuncExpn, IAzPattern
    {
        IAzTypeDefn CertainType { get; }

        public static IAzLiteralExpn Analyze(Scope scope,
                                             IPsLiteralExpn node)
        {
            return node switch
            {
                PsIntLiteral intLit => AzIntLiteral.Analyze(scope, intLit),
                PsRealLiteral realLit => AzRealLiteral.Analyze(scope, realLit),
                _ => throw new ArgumentException(),
            };
        }
    }

    public interface IAzLiteralExpn<T> : IAzLiteralExpn
    {
        T Value { get; }
    }
}
