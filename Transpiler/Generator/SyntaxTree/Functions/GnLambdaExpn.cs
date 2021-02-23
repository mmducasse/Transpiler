using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnLambdaExpn(GnParam Parameter,
                               IGnFuncExpn Expression) : IGnFuncExpn
    {
        public static GnLambdaExpn Prepare(IScope scope, AzLambdaExpn appExpn)
        {
            var param = GnParam.Prepare(scope, appExpn.Parameter);
            var expn = IGnFuncExpn.Prepare(scope, appExpn.Expression);

            return new(param, expn);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            string res = names.Next;
            string param = Parameter.Generate(i, names, ref s);
            s += string.Format("{0}var {1} = {2} => {{\n", Indent(i), res, param);

            string expnRes = Expression.Generate(i + 1, names, ref s);
            s += string.Format("{0}return {1}\n", Indent(i + 1), expnRes);
            s += string.Format("{0}}}\n", Indent(i));
            return res;
        }
    }
}
