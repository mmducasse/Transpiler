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
                PsElsePattern @else => AzElsePattern.Analyze(scope, @else),
                PsDectorPattern dector => AzDectorPattern.Analyze(scope, dector),
                PsTuplePattern tuple => AzTuplePattern.Analyze(scope, tuple),
                _ => throw new NotImplementedException(),
            };
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              IAzPattern node)
        {
            return node switch
            {
                AzElsePattern => ConstraintSet.Empty,
                AzDectorPattern dector => AzDectorPattern.Constrain(tvTable, scope, dector),
                AzTuplePattern tuple => AzTuplePattern.Constrain(tvTable, scope, tuple),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
