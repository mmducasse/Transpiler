using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzLiteralExpn : IAzFuncExpn, IAzPattern
    {
        IAzDataTypeDefn CertainType { get; }

        public static IAzFuncExpn Analyze(Scope scope,
                                             IPsLiteralExpn node)
        {
            return node switch
            {
                PsIntLiteral intLit => AzIntLiteral.Analyze(scope, intLit),
                PsRealLiteral realLit => AzRealLiteral.Analyze(scope, realLit),
                PsListLiteral listLit => AzListLiteral.Analyze(scope, listLit),
                _ => throw new ArgumentException(),
            };
        }
    }

    public interface IAzLiteralExpn<T> : IAzLiteralExpn
    {
        T Value { get; }
    }
}
