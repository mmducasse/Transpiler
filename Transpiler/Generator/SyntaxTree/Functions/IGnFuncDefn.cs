﻿using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnFuncDefn : IGnDefn
    {
    }

    public interface IGnFuncStmtDefn : IGnFuncDefn
    {
        bool InvokeImmediately { get; }

        string Generate(int i, NameProvider names, string namePrefix, ref string s);

        public static IGnFuncStmtDefn Prepare(Scope scope, IAzFuncStmtDefn defn)
        {
            return defn switch
            {
                AzFuncDefn funcDefn => GnFuncDefn.Prepare(scope, funcDefn),
                AzDectorFuncDefn funcDectorDefn => GnDectorFuncDefn.Prepare(scope, funcDectorDefn),
                _ => throw new Exception(),
            };
        }
    }
}
