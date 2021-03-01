using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public static class GnOperator
    {
        public static void Generate(Operator op, int i, ref string s)
        {
            s += string.Format("{0}function {1}(){{\n", Indent(i), op.Name.Generated());
            s += string.Format("{0}return {1}\n", Indent(i + 1), op.BackingFunctionName);
            s += Indent(i) + "}\n";
        }
    }
}
