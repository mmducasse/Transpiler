using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnRealLiteral(string Value) : IGnLiteralExpn
    {
        public static GnRealLiteral Prepare(IScope scope, AzRealLiteral realLit)
        {
            return new(realLit.Value);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            return string.Format("MkReal('{0}')", Value);
        }
    }
}
