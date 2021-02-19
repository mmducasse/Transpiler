using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzIfExpn(IAzFuncExpn Condition,
                           IAzFuncExpn ThenCase,
                           IAzFuncExpn ElseCase,
                           CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public static AzIfExpn Analyze(Scope scope,
                                       PsIfExpn ifExpn)
        {
            var condition = IAzFuncExpn.Analyze(scope, ifExpn.Condition);
            var thenCase = IAzFuncExpn.Analyze(scope, ifExpn.ThenCase);
            var elseCase = IAzFuncExpn.Analyze(scope, ifExpn.ElseCase);

            var newIfExpn = new AzIfExpn(condition, thenCase, elseCase, ifExpn.Position);
            return newIfExpn;
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            var csc = Condition.Constrain(provider, scope);
            var cst = ThenCase.Constrain(provider, scope);
            var cse = ElseCase.Constrain(provider, scope);

            var cif = new Constraint(Type, ThenCase.Type, this);
            var cc = new Constraint(Condition.Type, CoreTypes.Instance.Bool.ToCtor(), this);
            var cf = new Constraint(ThenCase.Type, ElseCase.Type, this);

            return IConstraintSet.Union(cif, cc, cf, csc, cst, cse);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Condition.GetSubnodes())
                               .Concat(ThenCase.GetSubnodes())
                               .Concat(ElseCase.GetSubnodes()).ToList();
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
