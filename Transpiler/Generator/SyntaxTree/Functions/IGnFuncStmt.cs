using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public interface IGnFuncStmt : IGnFuncNode
    {
        public static IGnFuncStmt Prepare(IScope scope, IAzFuncStmt funcStmt)
        {
            return funcStmt switch
            {
                IAzFuncExpn funcExpn => IGnFuncExpn.Prepare(scope, funcExpn),
                AzFuncDefn funcDefn => GnFuncDefn.Prepare(scope, funcDefn),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
