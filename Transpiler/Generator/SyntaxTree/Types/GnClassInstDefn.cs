﻿using System;
using System.Collections.Generic;
using System.Text;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public static class GnClassInstDefn
    {
        public static void Generate(Scope scope,
                                    AzClassInstDefn instDefn,
                                    ref StringBuilder output)
        {
            string s = "";

            List<string> dict = new();
            for (int x = 0; x < instDefn.Class.Superclasses.Count; x++)
            {
                string superClassName = instDefn.Class.Superclasses[x].Name;
                string superInstName = superClassName + "_" + instDefn.Implementor.Name;

                dict.Add(superInstName);
            }

            for (int i = 0; i < instDefn.Class.Functions.Count; i++)
            {
                var instFunc = instDefn.Functions[i];

                if (instFunc is IAzFuncStmtDefn funcStmtDefn &&
                    funcStmtDefn.Expression != null)
                {
                    var gnFunc = IGnFuncStmtDefn.Prepare(scope, funcStmtDefn);
                    string instFuncName = gnFunc.Generate(0, new("a"), instDefn.Implementor.Name, ref s);
                    s += "\n";

                    dict.Add(instFuncName + "()");
                }
                else if (instFunc is Operator op)
                {
                    dict.Add(op.BackingFunctionName);
                }
            }

            string dictName = instDefn.Class.Name + "_" + instDefn.Implementor.Name;
            s += string.Format("const {0} = [\n", dictName);
            s += dict.Separate(",\n");
            s += "]\n\n";

            output.Append(s);
        }
    }
}
