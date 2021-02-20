using System;
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
            for (int i = 0; i < classDefn.Functions.Count; i++)
            {
                string funcName = classDefn.Functions[i].Name.SafeName();
                string s = string.Format("function {0}(dict)\n", funcName);
                s += "{\n";
                s += string.Format("{0}return dict[{1}]\n", Indent(1), i);
                s += "}\n\n";

                output.Append(s);
            }
        }
    }
}
