using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzPattern : IAzFuncNode
    {
        public static IAzPattern Analyze(Scope scope,
                                         NameProvider provider,
                                         IPsPattern node)
        {
            return node switch
            {
                PsAnyPattern @else => AzElsePattern.Analyze(scope, provider, @else),
                PsParam param => AzParam.Analyze(scope, provider, param),
                PsDectorPattern dector => AzDectorPattern.Analyze(scope, provider, dector),
                PsTuplePattern tuple => AzTuplePattern.Analyze(scope, provider, tuple),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, provider, litExpn) as IAzPattern,
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, provider, arbExpn) as IAzPattern,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
