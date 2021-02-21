using System.Collections.Generic;
using Transpiler.Analysis;
using System;
using System.Linq;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnMatchExpn(IGnFuncExpn Argument,
                              IReadOnlyList<GnMatchCase> Cases) : IGnFuncExpn
    {
        public static GnMatchExpn Prepare(IScope scope, AzMatchExpn matchExpn)
        {
            var arg = IGnFuncExpn.Prepare(scope, matchExpn.Argument);
            var cases = matchExpn.Cases.Select(c => GnMatchCase.Prepare(scope, c)).ToList();

            return new(arg, cases);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            string arg = Argument.Generate(i, names, ref s);
            string res = names.Next;

            s += string.Format("{0}var {1};\n", Indent(i), res);

            bool isFirst = true;
            foreach (var @case in Cases)
            {
                @case.Generate(i, isFirst, arg, res, names, ref s);
                isFirst = false;
            }

            return res;
        }
    }
}
