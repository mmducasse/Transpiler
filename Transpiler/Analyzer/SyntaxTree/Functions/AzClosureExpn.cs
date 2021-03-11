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
                                Scope Scope,
                                CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzClosureExpn Analyze(Scope parentScope,
                                            NameProvider names,
                                            PsClosureExpn psCloseExpn)
        {
            var scope = new Scope(parentScope, "<Closure>");

            List<IAzFuncStmtDefn> statements = new();
            foreach (var s in psCloseExpn.Statements)
            {
                switch (s)
                {
                    case IPsFuncExpn funcExpn:
                        var expn = IAzFuncExpn.Analyze(scope, names, funcExpn);
                        var funcDefn = new AzFuncDefn(names.Next, expn.Type, eFixity.Prefix, true, Null)
                        {
                            InvokeImmediately = true
                        };
                        funcDefn.Expression = expn;
                        statements.Add(funcDefn);
                        break;
                    case IPsFuncStmtDefn psFuncStmtDefn:
                        var azFuncStmtDefns = IAzFuncStmtDefn.Initialize(scope, psFuncStmtDefn);
                        foreach (var fn in azFuncStmtDefns)
                        {
                            IAzFuncStmtDefn.Analyze(scope, names, fn, psFuncStmtDefn);
                            statements.Add(fn);
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            var returnExpn = IAzFuncExpn.Analyze(scope, names, psCloseExpn.ReturnExpression);
            return new(statements, returnExpn, scope, psCloseExpn.Position);
        }

        public ConstraintSet Constrain()
        {
            var cs = new ConstraintSet();

            foreach (var stmt in Statements)
            {
                var fcs = stmt.Constrain();
                cs = IConstraintSet.Union(fcs, cs);
            }

            var csr = ReturnExpression.Constrain();
            var creturn = new Constraint(Type, ReturnExpression.Type, Position);

            return IConstraintSet.Union(creturn, cs, csr);
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Statements.Foreach(s => s.Recurse(action));
            ReturnExpression.Recurse(action);
            action(this);
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
