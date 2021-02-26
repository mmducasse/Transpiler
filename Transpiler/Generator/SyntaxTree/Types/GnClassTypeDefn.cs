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
            int i = 0;
            for (; i < classDefn.Superclasses.Count(); i++)
            {
                string fnName = string.Format("{0}From{1}", classDefn.Superclasses[i].Name, classDefn.Name);
                output.Append(GenerateFunction(fnName, i));
            }

            for (int j = 0; j < classDefn.Functions.Count; j++)
            {
                string fnName = classDefn.Functions[j].Name;
                output.Append(GenerateFunction(fnName, i + j));
            }
        }

        private static string GenerateFunction(string name, int i)
        {
            string s = string.Format("function {0}()\n", name.SafeNameGenerated());
            s += "{\n";
            s += string.Format("{0}return dict => dict[{1}];\n", Indent(1), i);
            s += "}\n\n";

            return s;
        }
    }
}
