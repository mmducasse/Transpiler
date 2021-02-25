using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnUndefinedLiteral() : IGnLiteralExpn
    {
        public static GnUndefinedLiteral Prepare()
        {
            return new();
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            return "Undefined()";
        }
    }
}
