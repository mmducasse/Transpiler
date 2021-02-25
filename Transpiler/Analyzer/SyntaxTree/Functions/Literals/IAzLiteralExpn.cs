using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzLiteralExpn : IAzFuncExpn, IAzPattern
    {
        IAzTypeDefn CertainType { get; }

        string Value { get; }

        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider provider,
                                          IPsLiteralExpn node)
        {
            return node switch
            {
                PsIntLiteral intLit => AzIntLiteral.Analyze(scope, provider, intLit),
                PsRealLiteral realLit => AzRealLiteral.Analyze(scope, provider, realLit),
                PsCharLiteral charLit => AzCharLiteral.Analyze(scope, provider, charLit),
                PsStringLiteral stringLit => AzStringLiteral.Analyze(scope, provider, stringLit),
                PsListLiteral listLit => AzListLiteral.Analyze(scope, provider, listLit),
                PsUndefinedLiteral undefLit => AzUndefinedLiteral.Analyze(scope, provider, undefLit),
                _ => throw new ArgumentException(),
            };
        }
    }
}
