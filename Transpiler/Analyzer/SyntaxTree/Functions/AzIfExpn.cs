using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzIfExpn(IAzFuncExpn Condition,
                           IAzFuncExpn ThenCase,
                           IAzFuncExpn ElseCase,
                           CodePosition Position) : IAzFuncExpn
    {
        public static AzIfExpn Analyze(Scope scope,
                                       PsIfExpn ifExpn)
        {
            var condition = IAzFuncExpn.Analyze(scope, ifExpn.Condition);
            var thenCase = IAzFuncExpn.Analyze(scope, ifExpn.ThenCase);
            var elseCase = IAzFuncExpn.Analyze(scope, ifExpn.ElseCase);

            var newIfExpn = new AzIfExpn(condition, thenCase, elseCase, ifExpn.Position);
            return newIfExpn;
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzIfExpn node)
        {
            var csc = IAzFuncExpn.Constrain(tvTable, scope, node.Condition);
            var cst = IAzFuncExpn.Constrain(tvTable, scope, node.ThenCase);
            var cse = IAzFuncExpn.Constrain(tvTable, scope, node.ElseCase);

            var tif = tvTable.GetTypeOf(node);
            var tc = tvTable.GetTypeOf(node.Condition);
            var tt = tvTable.GetTypeOf(node.ThenCase);
            var te = tvTable.GetTypeOf(node.ElseCase);

            var cif = new Constraint(tif, tt, node);
            var cc = new Constraint(tc, CoreTypes.Instance.Bool.ToCtor(), node);
            var cf = new Constraint(tt, te, node);

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
