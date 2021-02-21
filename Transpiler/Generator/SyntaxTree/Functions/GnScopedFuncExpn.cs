using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnScopedFuncExpn(IGnFuncExpn Expression,
                                   IReadOnlyList<GnFuncDefn> FuncDefinitions,
                                   IScope Scope) : IGnFuncExpn
    {
        public static GnScopedFuncExpn Prepare(IScope scope, AzScopedFuncExpn scopedExpn)
        {
            var expn = IGnFuncExpn.Prepare(scope, scopedExpn.Expression);
            var fns = scopedExpn.FuncDefinitions.Select(f => GnFuncDefn.Prepare(scope, f)).ToList();

            return new(expn, fns, scopedExpn.Scope);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            throw new System.Exception();

            //s += string.Format("{0}{{\n", Indent(i));
            //foreach (var fn in FuncDefinitions)
            //{
            //    fn.Generate(i + 1, names, ref s);
            //    s += "\n";
            //}
            //string expnRes = Expression.Generate(i, names, ref s);
            //s += string.Format("\n{0}return {1} \n", Indent(i + 1), expnRes);
            //s += "}}\n";
            //return s;
        }
    }
}
