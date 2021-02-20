using System;
using System.Collections.Generic;
using System.Text;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public static class GnClassInstDefn
    {
        public static void Generate(AzClassInstDefn instDefn, ref StringBuilder output)
        {
            List<string> dict = new();
            for (int i = 0; i < instDefn.Class.Functions.Count; i++)
            {
                var instFunc = instDefn.Functions[i];

                string g = "";
                if (instFunc is AzFuncDefn funcDefn &&
                    funcDefn.Expression != null)
                {
                    var gnFunc = GnFuncDefn.Prepare(funcDefn);
                    string instFuncName = gnFunc.Generate(0, new(), instDefn.Implementor.Name, ref g);

                    dict.Add(instFuncName);
                }
                else if (instFunc is Operator op)
                {
                    dict.Add(op.BackingFunctionName);
                }
            }

            string dictName = instDefn.Class.Name.Generated() + instDefn.Implementor.Name.Generated();
            string s = string.Format("const {0} = [\n", dictName);
            s += dict.Separate(",\n");
            s += "]\n\n";

            output.Append(s);
        }
    }
}
