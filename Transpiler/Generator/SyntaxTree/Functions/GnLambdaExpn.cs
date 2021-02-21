using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnLambdaExpn(GnParam Parameter,
                               IGnFuncExpn Expression) : IGnFuncExpn
    {
        public static GnLambdaExpn Prepare(IScope scope, AzLambdaExpn appExpn)
        {
            // Todo: Handle tpye arguemnts.

            var param = GnParam.Prepare(scope, appExpn.Parameter);
            var expn = IGnFuncExpn.Prepare(scope, appExpn.Expression);

            return new(param, expn);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            s += Parameter.Generate(i, names, ref s);
            s += " => ";
            if (Expression is GnScopedFuncExpn scopedExpn)
            {
                s += "\n";
                s += Indent(i) + "{\n";
                foreach (var fn in scopedExpn.FuncDefinitions)
                {
                    fn.Generate(i + 1, names, ref s);
                    s += "\n";
                }
                string expnRes = scopedExpn.Expression.Generate(i + 1, names, ref s);
                s += string.Format("{0}return {1}\n", Indent(i + 1), expnRes);
                s += Indent(i) + "}\n";
            }
            else
            {
                Expression.Generate(i, names, ref s);
            }
            return null;
        }
    }
}
