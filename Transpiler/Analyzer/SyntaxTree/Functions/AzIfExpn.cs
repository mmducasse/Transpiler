using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzIfExpn(IAzFuncExpn Condition,
                           AzScopedFuncExpn ThenCase,
                           AzScopedFuncExpn ElseCase,
                           CodePosition Position) : IAzFuncExpn
    {
        public static AzIfExpn Analyze(Scope scope,
                                       PsIfExpn ifExpn)
        {
            var condition = IAzFuncExpn.Analyze(scope, ifExpn.Condition);
            var thenCase = AzScopedFuncExpn.Analyze(scope, ifExpn.ThenCase);
            var elseCase = AzScopedFuncExpn.Analyze(scope, ifExpn.ElseCase);

            var newIfExpn = new AzIfExpn(condition, thenCase, elseCase, ifExpn.Position);
            return newIfExpn;
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzIfExpn node)
        {
            var csc = IAzFuncExpn.Constrain(tvTable, scope, node.Condition);
            var cst = AzScopedFuncExpn.Constrain(tvTable, node.ThenCase);
            var cse = AzScopedFuncExpn.Constrain(tvTable, node.ElseCase);

            var tif = tvTable.GetTypeOf(node);
            var tc = tvTable.GetTypeOf(node.Condition);
            var tt = tvTable.GetTypeOf(node.ThenCase.Expression);
            var te = tvTable.GetTypeOf(node.ElseCase.Expression);

            var cif = new EqualConstraint(tif, tt, node);
            var cc = new EqualConstraint(tc, CoreTypes.Instance.Bool.ToCtor(), node);
            var cf = new EqualConstraint(tt, te, node);

            return IConstraintSet.Union(cif, cc, cf, csc, cst, cse);
        }

        public string Print(int i)
        {
            int i1 = i + 1;
            string s = string.Format("if {0}\n", Condition.Print(i));
            s += string.Format("{0}then {1}\n", Indent(i1), ThenCase.Print(i1));
            s += string.Format("{0}else {1}\n", Indent(i1), ElseCase.Print(i1));

            return s;
        }

        public override string ToString() => Print(0);
    }
}
