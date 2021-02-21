using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnPattern : IGnFuncNode, IGnInlineNode
    {
        public static IGnPattern Prepare(IScope scope, IAzPattern pattern)
        {
            return pattern switch
            {
                IAzLiteralExpn litExpn => IGnLiteralExpn.Prepare(scope, litExpn),
                AzSymbolExpn symExpn => GnSymbolExpn.Prepare(scope, symExpn),
                AzElsePattern @else => GnElsePattern.Prepare(scope, @else),
                AzParam param => GnParam.Prepare(scope, param),
                AzDectorPattern dector => GnDectorPattern.Prepare(scope, dector),
                AzTuplePattern tuple => GnTuplePattern.Prepare(scope, tuple),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public interface IGnDectorPattern : IGnPattern
    {
        void GenerateAccessors(int i, string arg, NameProvider names, ref string s);
    }
}
