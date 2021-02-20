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
                PsAnyPattern @else => AzElsePattern.Analyze(scope, @else),
                PsParam param => AzParam.Analyze(scope, param),
                PsDectorPattern dector => AzDectorPattern.Analyze(scope, dector),
                PsTuplePattern tuple => AzTuplePattern.Analyze(scope, tuple),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, litExpn) as IAzPattern,
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, arbExpn) as IAzPattern,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
