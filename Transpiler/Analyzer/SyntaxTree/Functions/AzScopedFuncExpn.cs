using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzScopedFuncExpn(IAzFuncExpn Expression,
                                   IReadOnlyList<IAzFuncStmtDefn> FuncDefinitions,
                                   Scope Scope,
                                   CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzScopedFuncExpn Make(IAzFuncExpn expression,
                                            Scope scope,
                                            IReadOnlyList<IAzFuncStmtDefn> funcDefinitions = null,
                                            CodePosition position = null)
        {
            return new(expression, funcDefinitions ?? RList<IAzFuncStmtDefn>(), scope, position ?? null);
        }

        public static IAzFuncExpn Analyze(Scope parentScope,
                                          NameProvider names,
                                          PsScopedFuncExpn scopedExpn)
        {
            var scope = new Scope(parentScope);

            var newSubDefns = Analyzer.AnalyzeFunctions(scope, scopedExpn.FuncDefinitions);

            if (scopedExpn.Expression is PsMatchExpn matchExpn &&
                matchExpn.IsTerse)
            {
                // This will return a lambda with a scoped expn at it's tail.
                return IAzFuncExpn.Analyze(scope, names, scopedExpn.Expression);
            }
            else
            {
                var newExpn = IAzFuncExpn.Analyze(scope, names, scopedExpn.Expression);

                return new AzScopedFuncExpn(newExpn, newSubDefns, scope, scopedExpn.Position);
            }
        }

        public ConstraintSet Constrain()
        {
            var cs = new ConstraintSet();

            foreach (var fn in FuncDefinitions)
            {
                var fcs = fn.Constrain();
                cs = IConstraintSet.Union(fcs, cs);
            }

            var cse = Expression.Constrain();
            var ccc = new Constraint(Type, Expression.Type, Position);

            return IConstraintSet.Union(cse, cs, ccc);
        }

        public string Print(int i)
        {
            string s = "[" + Expression.Print(i);
            foreach (var subDefn in FuncDefinitions)
            {
                s += string.Format("\n{0}{1}", Indent(i + 1), subDefn.Print(i + 1));
            }

            return s + "]";
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Expression.Recurse(action);
            FuncDefinitions.Foreach(f => f.Recurse(action));
            action(this);
        }

        public override string ToString() => Print(0);
    }
}
