using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnGetElementExpn(int ElementIndex,
                                   IGnFuncExpn Expression) : IGnFuncExpn
    {
        public static GnGetElementExpn Prepare(IScope scope, AzGetElementExpn getExpn)
        {
            var expn = IGnFuncExpn.Prepare(scope, getExpn.Expression);
            return new(getExpn.ElementIndex, expn);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            string expnRes = Expression.Generate(i, names, ref s);
            string res = names.Next;
            s += string.Format("{0}{1} = Get({2}, {3})\n", Indent(i), res, ElementIndex, expnRes);
            return res;
        }
    }
}
