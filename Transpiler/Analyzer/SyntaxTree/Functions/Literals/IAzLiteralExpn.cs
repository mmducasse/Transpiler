﻿using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzLiteralExpn : IAzFuncExpn, IAzPattern
    {
        IAzTypeDefn CertainType { get; }

        string Value { get; }

        public static IAzFuncExpn Analyze(Scope scope,
                                          IPsLiteralExpn node)
        {
            return node switch
            {
                PsIntLiteral intLit => AzIntLiteral.Analyze(scope, intLit),
                PsRealLiteral realLit => AzRealLiteral.Analyze(scope, realLit),
                PsCharLiteral charLit => AzCharLiteral.Analyze(scope, charLit),
                PsStringLiteral stringLit => AzStringLiteral.Analyze(scope, stringLit),
                PsListLiteral listLit => AzListLiteral.Analyze(scope, listLit),
                PsUndefinedLiteral undefLit => AzUndefinedLiteral.Analyze(scope, undefLit),
                _ => throw new ArgumentException(),
            };
        }
    }
}
