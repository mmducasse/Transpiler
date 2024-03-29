﻿using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnIfExpn(IGnFuncExpn Condition,
                           IGnFuncExpn ThenCase,
                           IGnFuncExpn ElseCase) : IGnFuncExpn
    {
        public static GnIfExpn Prepare(IScope scope, AzIfExpn funcDefn)
        {
            var cond = IGnFuncExpn.Prepare(scope, funcDefn.Condition);
            var then = IGnFuncExpn.Prepare(scope, funcDefn.ThenCase);
            var @else = IGnFuncExpn.Prepare(scope, funcDefn.ElseCase);

            return new(cond, then, @else);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {

            string res = names.Next;
            s += string.Format("{0}var {1};\n", Indent(i), res);

            var condRes = Condition.Generate(i, names, ref s);

            s += string.Format("{0}if (Match({1}, ['True']))\n", Indent(i), condRes);
            GenerateCase(ThenCase, res, i, names, ref s);

            s += string.Format("{0}else\n", Indent(i));
            GenerateCase(ElseCase, res, i, names, ref s);

            return res;
        }

        private static void GenerateCase(IGnFuncExpn @case,
                                         string res,
                                         int i,
                                         NameProvider names,
                                         ref string s)
        {
            s += Indent(i) + "{\n";
            string expnRes = @case.Generate(i + 1, names, ref s);
            s += string.Format("{0}{1} = {2}\n", Indent(i + 1), res, expnRes);
            s += Indent(i) + "}\n";
        }
    }
}
