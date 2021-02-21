using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnParam(string Name) : IGnPattern, IGnFuncDefn
    {
        public static GnParam Prepare(IScope scope, AzParam param)
        {
            return new(param.Name);
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
