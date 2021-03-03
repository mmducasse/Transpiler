using System;
using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public interface IAzFuncNode : IAzNode
    {
        IAzTypeExpn Type { get; }

        ConstraintSet Constrain();

        void SubstituteType(Substitution s);

        void Recurse(Action<IAzFuncNode> action);
    }
}
