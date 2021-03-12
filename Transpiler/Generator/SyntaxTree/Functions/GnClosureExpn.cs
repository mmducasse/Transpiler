using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnClosureExpn(IReadOnlyList<IGnFuncStmtDefn> Statements,
                                IGnFuncExpn ReturnExpression,
                                IScope Scope) : IGnFuncExpn
    {
        public static GnClosureExpn Prepare(AzClosureExpn closeExpn)
        {
            var stmts = closeExpn.Statements.Select(s => IGnFuncStmtDefn.Prepare(closeExpn.Scope, s)).ToList();
            var retExpn = IGnFuncExpn.Prepare(closeExpn.Scope, closeExpn.ReturnExpression);

            return new(stmts, retExpn, closeExpn.Scope);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            foreach (var stmt in Statements)
            {
                string stmtRes = stmt.Generate(i, names, ref s);
                if ((stmt is IGnFuncExpn) && stmt.InvokeImmediately)
                {
                    s += string.Format("{0}{1}()\n", Indent(i), stmtRes);
                }
            }
            return ReturnExpression.Generate(i, names, ref s);
        }
    }
}
