using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnElsePattern() : IGnPattern
    {
        public static GnElsePattern Prepare(IScope scope, AzElsePattern pattern)
        {
            return new();
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            return "null";
        }
    }
}
