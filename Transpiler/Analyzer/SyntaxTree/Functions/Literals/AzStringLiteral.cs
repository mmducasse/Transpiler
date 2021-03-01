using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class AzStringLiteral
    {
        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          PsStringLiteral stringLiteral)
        {
            var chars = stringLiteral.Value.Select(c => new AzCharLiteral(c.ToString(), stringLiteral.Position)).ToList();

            return AzListLiteral.CreateList(scope, tvs, chars);
        }
    }
}
