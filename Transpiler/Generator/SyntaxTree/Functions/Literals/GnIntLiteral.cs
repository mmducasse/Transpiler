using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnIntLiteral(string Value) : IGnLiteralExpn
    {
        public static GnIntLiteral Prepare(IScope scope, AzIntLiteral intLit)
        {
            return new(intLit.Value);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            return string.Format("MkInt('{0}')", Value);
        }
    }
}
