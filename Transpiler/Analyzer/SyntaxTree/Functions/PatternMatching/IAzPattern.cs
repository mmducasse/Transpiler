using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzPattern : IAzFuncNode
    {
        public static IAzPattern Analyze(Scope scope,
                                         IPsPattern node)
        {
            return node switch
            {
                PsDectorPattern dector => AzDectorPattern.Analyze(scope, dector),
                _ => throw new NotImplementedException(),
            };
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              IAzPattern node)
        {
            return node switch
            {
                AzDectorPattern dector => AzDectorPattern.Constrain(tvTable, scope, dector),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
