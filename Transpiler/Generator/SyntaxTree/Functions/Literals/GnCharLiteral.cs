using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnCharLiteral(string Value) : IGnLiteralExpn
    {
        public static GnCharLiteral Prepare(AzCharLiteral charLit)
        {
            return new(charLit.Value);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            return string.Format("'{0}'", Value);
        }
    }
}
