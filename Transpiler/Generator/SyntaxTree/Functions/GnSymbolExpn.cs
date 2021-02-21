using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnSymbolExpn(string Name) : IGnFuncExpn, IGnPattern, IGnInlineNode
    {
        public static GnSymbolExpn Prepare(IScope scope, AzSymbolExpn symExpn)
        {
            return new(symExpn.Definition.Name);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            return Name.SafeName();
        }
    }
}
