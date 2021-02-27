using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncDefn : IAzDefn, IAzFuncNode
    {
        eFixity Fixity { get; }

        IAzTypeExpn ExplicitType { get; }
    }

    public interface IAzFuncStmtDefn : IAzFuncDefn, IAzFuncStmt
    {
        new IAzTypeExpn ExplicitType { get; set; }

        IAzFuncExpn Expression { get; }

        public static IReadOnlyList<IAzFuncStmtDefn> Initialize(Scope scope,
                                                                IPsFuncStmtDefn node)
        {
            return node switch
            {
                PsFuncDefn funcDefn => AzFuncDefn.Initialize(scope, funcDefn).ToArr(),
                PsDectorFuncDefn dectorFuncDefn => AzDectorFuncDefn.Initialize(scope, dectorFuncDefn),
                _ => throw new System.Exception(),
            };
        }

        public static IAzFuncDefn Analyze(Scope parentScope,
                                          NameProvider provider,
                                          IAzFuncStmtDefn funcDefn,
                                          IPsFuncStmtDefn node)
        {
            return (funcDefn, node) switch
            {
                (AzFuncDefn azFuncDefn, PsFuncDefn psFuncDefn) =>
                    AzFuncDefn.Analyze(parentScope, provider, azFuncDefn, psFuncDefn),
                (AzDectorFuncDefn azDectorFuncDefn, PsDectorFuncDefn psDectorFuncDefn) =>
                    AzDectorFuncDefn.Analyze(parentScope, provider, azDectorFuncDefn, psDectorFuncDefn),
                _ => throw new System.Exception(),
            };
        }
    }

}
