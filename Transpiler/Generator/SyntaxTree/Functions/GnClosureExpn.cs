using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnClosureExpn(IReadOnlyList<IGnFuncStmt> Statements,
                                IGnFuncExpn ReturnExpression,
                                IScope Scope) : IGnFuncExpn
    {
        public static GnClosureExpn Prepare(AzClosureExpn closeExpn)
        {
            var stmts = closeExpn.Statements.Select(s => IGnFuncStmt.Prepare(closeExpn.Scope, s)).ToList();
            var retExpn = IGnFuncExpn.Prepare(closeExpn.Scope, closeExpn.ReturnExpression);

            return new(stmts, retExpn, closeExpn.Scope);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            foreach (var stmt in Statements)
            {
                stmt.Generate(i, names, ref s);
            }
            return ReturnExpression.Generate(i, names, ref s);
        }
    }
}
