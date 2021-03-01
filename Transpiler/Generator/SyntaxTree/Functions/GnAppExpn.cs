using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnAppExpn(IGnFuncExpn Function,
                            IGnFuncExpn Argument) : IGnFuncExpn
    {
        public static GnAppExpn Prepare(IScope scope, AzAppExpn appExpn)
        {
            var func = IGnFuncExpn.Prepare(scope, appExpn.Function);
            var arg = IGnFuncExpn.Prepare(scope, appExpn.Argument);

            return new(func, arg);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            string func;
            if (Function is IGnInlineNode funcSym)
            {
                func = funcSym.Generate();
            }
            else
            {
                func = Function.Generate(i, names, ref s);
            }

            string arg;
            if (Argument is IGnInlineNode argSym)
            {
                arg = argSym.Generate();
            }
            else
            {
                arg = Argument.Generate(i, names, ref s);
            }

            string appRes = names.Next;
            s += string.Format("{0}var {1} = {2}({3})\n", Indent(i), appRes, func, arg);
            return appRes;
        }
    }
}
