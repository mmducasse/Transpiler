using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzPattern : IAzFuncNode
    {
        public static IAzPattern Analyze(Scope scope,
                                         NameProvider names,
                                         IPsPattern node)
        {
            return node switch
            {
                PsAnyPattern @else => AzElsePattern.Analyze(@else),
                PsParam param => AzParam.Analyze(scope, names, param),
                PsDectorPattern dector => AzDectorPattern.Analyze(scope, names, dector),
                PsTuplePattern tuple => AzTuplePattern.Analyze(scope, names, tuple),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, names, litExpn) as IAzPattern,
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, names, arbExpn) as IAzPattern,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
