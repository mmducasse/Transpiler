using System;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnPattern : IGnFuncNode, IGnInlineNode
    {
        public static IGnPattern Prepare(IAzPattern pattern)
        {
            return pattern switch
            {
                AzElsePattern @else => GnElsePattern.Prepare(@else),
                AzParam param => GnParam.Prepare(param),
                AzDectorPattern dector => GnDectorPattern.Prepare(dector),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
