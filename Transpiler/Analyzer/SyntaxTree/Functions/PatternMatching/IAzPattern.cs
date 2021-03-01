using System;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzPattern : IAzFuncNode
    {
        IAzPattern SubstituteType(Substitution s);

        public static IAzPattern Analyze(Scope scope,
                                         NameProvider names,
                                         TvProvider tvs,
                                         IPsPattern node)
        {
            return node switch
            {
                PsAnyPattern @else => AzElsePattern.Analyze(tvs, @else),
                PsParam param => AzParam.Analyze(scope, names, tvs, param),
                PsDectorPattern dector => AzDectorPattern.Analyze(scope, names, tvs, dector),
                PsTuplePattern tuple => AzTuplePattern.Analyze(scope, names, tvs, tuple),
                IPsLiteralExpn litExpn => IAzLiteralExpn.Analyze(scope, names, tvs, litExpn) as IAzPattern,
                PsArbExpn arbExpn => AzAppExpn.Analyze(scope, names, tvs, arbExpn) as IAzPattern,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
