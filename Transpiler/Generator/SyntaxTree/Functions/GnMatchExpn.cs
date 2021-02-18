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
        public static GnMatchExpn Prepare(AzMatchExpn matchExpn)
        {
            var arg = IGnFuncExpn.Prepare(matchExpn.Argument);
            var cases = matchExpn.Cases.Select(c => GnMatchCase.Prepare(c)).ToList();

            return new(arg, cases);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            string arg = Argument.Generate(i, names, ref s);
            string res = names.Next;

            s += string.Format("{0}var {1};\n", Indent(i), res);

            foreach (var @case in Cases)
            {
                @case.Generate(i, arg, res, names, ref s);
            }

            return res;
        }
    }
}
