using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzLiteralExpn : IAzFuncExpn, IAzPattern
    {
        IAzTypeDefn CertainType { get; }

        string Value { get; }

        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          IPsLiteralExpn node)
        {
            return node switch
            {
                PsIntLiteral intLit => AzIntLiteral.Analyze(intLit),
                PsRealLiteral realLit => AzRealLiteral.Analyze(realLit),
                PsCharLiteral charLit => AzCharLiteral.Analyze(charLit),
                PsUndefinedLiteral undefLit => AzUndefinedLiteral.Analyze(undefLit),
                PsStringLiteral stringLit => AzStringLiteral.Analyze(scope, names, tvs, stringLit),
                PsListLiteral listLit => AzListLiteral.Analyze(scope, names, tvs, listLit),
                _ => throw new ArgumentException(),
            };
        }
    }
}
