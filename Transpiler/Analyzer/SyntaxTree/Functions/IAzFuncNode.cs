using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public interface IAzFuncNode : IAzNode
    {
        IAzTypeExpn Type { get; }

        ConstraintSet Constrain(TvProvider provider, Scope scope);

        IReadOnlyList<IAzFuncNode> GetSubnodes();
    }
}
