using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzScopedFuncExpn(IAzFuncExpn Expression,
                                   IReadOnlyList<IAzFuncStmtDefn> FuncDefinitions,
                                   IAzTypeExpn Type,
                                   Scope Scope,
                                   CodePosition Position) : IAzFuncExpn
    {
        //public static AzScopedFuncExpn Make(IAzFuncExpn expression) =>
        //    new(expression, new List<PsFuncDefn>());

        public static IAzFuncExpn Analyze(Scope parentScope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          PsScopedFuncExpn scopedExpn)
        {
            var scope = new Scope(parentScope);

            var newSubDefns = Analyzer.AnalyzeFunctions(scope, tvs, scopedExpn.FuncDefinitions);

            if (scopedExpn.Expression is PsMatchExpn matchExpn &&
                matchExpn.IsTerse)
            {
                // This will return a lambda with a scoped expn at it's tail.
                return IAzFuncExpn.Analyze(scope, names, tvs, scopedExpn.Expression);
            }
            else
            {
                var newExpn = IAzFuncExpn.Analyze(scope, names, tvs, scopedExpn.Expression);

                return new AzScopedFuncExpn(newExpn, newSubDefns, tvs.Next, scope, scopedExpn.Position);
            }
        }

        public ConstraintSet Constrain(TvProvider provider, Scope _)
        {
            var cs = new ConstraintSet();

            foreach (var fn in FuncDefinitions)
            {
                var fcs = fn.Constrain(provider, Scope);
                cs = IConstraintSet.Union(fcs, cs);
            }

            var cse = Expression.Constrain(provider, Scope);
            var ccc = new Constraint(Type, Expression.Type, Position);

            return IConstraintSet.Union(cse, cs, ccc);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var elementNodes = FuncDefinitions.SelectMany(f => f.GetSubnodes()).ToList();
            return this.ToArr().Concat(Expression.GetSubnodes()).Concat(elementNodes).ToList();
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

        public IAzFuncExpn SubstituteType(Substitution s)
        {
            return new AzScopedFuncExpn(Expression.SubstituteType(s),
                                        FuncDefinitions.Select(f => f.SubstituteType(s)).ToList(),
                                        Type.Substitute(s),
                                        Scope,
                                        Position);
        }

        public override string ToString() => Print(0);
    }
}
