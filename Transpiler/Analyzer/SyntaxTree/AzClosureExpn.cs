using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzClosureExpn(IReadOnlyList<IAzFuncStmt> Statements,
                                IAzFuncExpn ReturnExpression,
                                Scope Scope,
                                CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public static AzClosureExpn Analyze(Scope parentScope,
                                            PsClosureExpn node)
        {
            var scope = new Scope(parentScope, "Closure");

            List<IAzFuncStmt> statements = new();
            foreach (var s in node.Statements)
            {
                switch (s)
                {
                    case IPsFuncExpn funcExpn:
                        statements.Add(IAzFuncExpn.Analyze(scope, funcExpn));
                        break;
                    case PsFuncDefn funcDefn:
                        var funcDefns = AzFuncDefn.Initialize(scope, funcDefn);
                        foreach (var fn in funcDefns)
                        {
                            AzFuncDefn.Analyze(scope, fn, funcDefn);
                            statements.Add(fn);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var returnExpn = IAzFuncExpn.Analyze(scope, node.ReturnExpression);

            return new(statements, returnExpn, scope, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope _)
        {
            var cs = new ConstraintSet();

            foreach (var stmt in Statements)
            {
                var fcs = stmt.Constrain(provider, Scope);
                cs = IConstraintSet.Union(fcs, cs);
            }

            var csr = ReturnExpression.Constrain(provider, Scope);
            Type = ReturnExpression.Type;

            return IConstraintSet.Union(cs, csr);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Statements.Concat(ReturnExpression.ToArr())).ToList();
        }

        public string Print(int i)
        {
            string s = "{\n";
            foreach (var stmt in Statements)
            {
                s += Indent(i + 1) + stmt.Print(i + 1) + "\n";
            }
            s += Indent(i + 1) + ReturnExpression.Print(i + 1) + "\n";
            return s;
        }

        public override string ToString() => Print(0);
    }
}
