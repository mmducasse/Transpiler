﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public static class GnClassTypeDefn
    {
        public static void Generate(AzClassTypeDefn classDefn, ref StringBuilder output)
        {
            int i = 0;
            for (; i < classDefn.Superclasses.Count(); i++)
            {
                string fnName = string.Format("{0}From{1}", classDefn.Superclasses[i].Name, classDefn.Name);
                output.Append(GenerateFunction(fnName, i, false));
            }

            for (int j = 0; j < classDefn.Functions.Count; j++)
            {
                string fnName = classDefn.Functions[j].Name;
                output.Append(GenerateFunction(fnName, i + j, true));
            }
        }

        private static string GenerateFunction(string name, int i, bool generate)
        {
            string genName = generate ? name.SafeNameGenerated() : name.SafeName();
            string s = string.Format("function {0}(dict)\n", genName);
            s += "{\n";
            s += string.Format("{0}return dict[{1}];\n", Indent(1), i);
            s += "}\n\n";

            return s;
        }
    }
}
