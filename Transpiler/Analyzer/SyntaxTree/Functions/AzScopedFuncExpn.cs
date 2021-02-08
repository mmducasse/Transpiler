using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzScopedFuncExpn(IAzFuncExpn Expression,
                                   IReadOnlyList<AzFuncDefn> FuncDefinitions,
                                   Scope Scope,
                                   CodePosition Position) : IAzFuncExpn
    {
        //public static AzScopedFuncExpn Make(IAzFuncExpn expression) =>
        //    new(expression, new List<PsFuncDefn>());

        public static AzScopedFuncExpn Analyze(Scope parentScope,
                                               PsScopedFuncExpn scopedExpn)
        {
            var scope = Scope.FunctionScope(parentScope);

            var newSubDefns = Analyzer.AnalyzeFunctions(scope, scopedExpn.FuncDefinitions);

            var newExpn = IAzFuncExpn.Analyze(scope, scopedExpn.Expression);

            return new(newExpn, newSubDefns, scope, scopedExpn.Position);
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              AzScopedFuncExpn node)
        {
            var cs = new ConstraintSet();

            foreach (var fn in node.FuncDefinitions)
            {
                var fcs = AzFuncDefn.Constrain(tvTable, node.Scope, fn);
                cs = IConstraints.Union(fcs, cs);
            }

            var cse = IAzFuncExpn.Constrain(tvTable, node.Scope, node.Expression);

            return IConstraints.Union(cse, cs);
        }

        public string Print(int i)
        {
            string s = Expression.Print(i);
            foreach (var subDefn in FuncDefinitions)
            {
                s += string.Format("\n{0}{1}", Indent(i + 1), subDefn.Print(i + 1));
            }

            return s;
        }
    }
}
