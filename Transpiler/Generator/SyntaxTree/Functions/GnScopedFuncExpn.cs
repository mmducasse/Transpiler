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
        public static GnScopedFuncExpn Prepare(AzScopedFuncExpn scopedExpn)
        {
            var expn = IGnFuncExpn.Prepare(scopedExpn.Scope, scopedExpn.Expression);
            var fns = scopedExpn.FuncDefinitions.Select(f => GnFuncDefn.Prepare(scopedExpn.Scope, f)).ToList();

            return new(expn, fns, scopedExpn.Scope);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            foreach (var fn in FuncDefinitions)
            {
                fn.Generate(i, names, ref s);
                s += "\n";
            }
            return Expression.Generate(i, names, ref s);
        }
    }
}
