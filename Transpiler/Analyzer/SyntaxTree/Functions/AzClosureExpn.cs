using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzClosureExpn(IReadOnlyList<IAzFuncStmtDefn> Statements,
                                IAzFuncExpn ReturnExpression,
                                IAzTypeExpn Type,
                                Scope Scope,
                                CodePosition Position) : IAzFuncExpn
    {
        public static AzClosureExpn Analyze(Scope parentScope,
                                            NameProvider names,
                                            TvProvider tvs,
                                            PsClosureExpn psCloseExpn)
        {
            var scope = new Scope(parentScope, "<Closure>");

            List<IAzFuncStmtDefn> statements = new();
            foreach (var s in psCloseExpn.Statements)
            {
                switch (s)
                {
                    case IPsFuncExpn funcExpn:
                        var expn = IAzFuncExpn.Analyze(scope, names, tvs, funcExpn);
                        var funcDefn = new AzFuncDefn(names.Next, expn.Type, eFixity.Prefix, true, Null);
                        statements.Add(funcDefn);
                        break;
                    case IPsFuncStmtDefn psFuncStmtDefn:
                        var azFuncStmtDefns = IAzFuncStmtDefn.Initialize(scope, psFuncStmtDefn);
                        foreach (var fn in azFuncStmtDefns)
                        {
                            IAzFuncStmtDefn.Analyze(scope, names, tvs, fn, psFuncStmtDefn);
                            statements.Add(fn);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var returnExpn = IAzFuncExpn.Analyze(scope, names, tvs, psCloseExpn.ReturnExpression);
            return new(statements, returnExpn, tvs.Next, scope, psCloseExpn.Position);
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
            var creturn = new Constraint(Type, ReturnExpression.Type, Position);

            return IConstraintSet.Union(creturn, cs, csr);
        }

        public IAzFuncExpn SubstituteType(Substitution s)
        {
            return new AzClosureExpn(Statements.Select(st => st.SubstituteType(s)).ToList(),
                                     ReturnExpression.SubstituteType(s),
                                     Type.Substitute(s),
                                     Scope,
                                     Position);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var statementNodes = Statements.SelectMany(f => f.GetSubnodes()).ToList();
            return this.ToArr().Concat(statementNodes).Concat(ReturnExpression.ToArr()).ToList();
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
