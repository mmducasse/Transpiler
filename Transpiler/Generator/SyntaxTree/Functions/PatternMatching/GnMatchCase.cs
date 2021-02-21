using System;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnMatchCase(IGnPattern Pattern,
                              IGnFuncExpn Expression) : IGnFuncNode
    {
        public static GnMatchCase Prepare(IScope scope, AzMatchCase matchCase)
        {
            var pattern = IGnPattern.Prepare(scope, matchCase.Pattern);
            var expn = IGnFuncExpn.Prepare(scope, matchCase.Expression);

            return new(pattern, expn);
        }

        public string Generate(int i, bool isFirstCase, string arg, string res, NameProvider names, ref string s)
        {
            string flowCtrl = isFirstCase ? "if" : "else if";
            s += string.Format("{0}{1} (Match({2}, {3}))\n", Indent(i), flowCtrl, arg, Pattern.Generate());
            s += Indent(i) + "{\n";

            if (Pattern is IGnDectorPattern dectorPattern)
            {
                dectorPattern.GenerateAccessors(i + 1, arg, names, ref s);
            }
            if (Expression is GnScopedFuncExpn scopedExpn)
            {
                foreach (var fn in scopedExpn.FuncDefinitions)
                {
                    fn.Generate(i + 1, names, ref s);
                    s += "\n";
                }
                string expnRes = scopedExpn.Expression.Generate(i + 1, names, ref s);
                s += string.Format("{0}{1} = {2}\n", Indent(i + 1), res, expnRes);
            }
            else
            {
                s += string.Format("{0}{1} = ", Indent(i + 1), res);
                Expression.Generate(i + 1, names, ref s);
            }
            s += Indent(i) + "}\n";

            return null;
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            throw new NotImplementedException();
        }
    }
}
