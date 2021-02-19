using System.Collections.Generic;
using Transpiler.Analysis;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public interface IGnFuncDefn : IGnDefn, IGnFuncNode
    {
    }

    // Todo: Add optional Type constraint property.
    public record GnFuncDefn(string Name,
                             IGnFuncExpn Expression) : IGnFuncDefn
    {
        public static GnFuncDefn Prepare(AzFuncDefn funcDefn)
        {
            var expn = IGnFuncExpn.Prepare(funcDefn.Expression);

            return new GnFuncDefn(funcDefn.Name, expn);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            if (Expression is GnScopedFuncExpn scopedExpn)
            {
                string name = Name.SafeName();
                string helperName = "c" + Name.SafeName();
                s += string.Format("{0}function {1}() {{\n", Indent(i), helperName);
                foreach (var fn in scopedExpn.FuncDefinitions)
                {
                    fn.Generate(i + 1, names, ref s);
                }
                string expnRes = scopedExpn.Expression.Generate(i + 1, names, ref s);
                s += string.Format("{0}return {1}\n", Indent(i + 1), expnRes);
                s += Indent(i) + "}\n";
                s += string.Format("{0}const {1} = {2}()\n", Indent(i), name, helperName);

                return s;
            }
            else
            {
                string name = Name.SafeName();
                s += string.Format("{0}const {1} = ", Indent(i), name);
                Expression.Generate(i, names, ref s);

                return null;
            }
        }
    }
}
