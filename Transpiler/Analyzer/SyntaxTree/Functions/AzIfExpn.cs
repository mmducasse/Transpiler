using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzIfExpn(IAzFuncExpn Condition,
                           IAzFuncExpn ThenCase,
                           IAzFuncExpn ElseCase,
                           IAzTypeExpn Type,
                           CodePosition Position) : IAzFuncExpn
    {
        public static AzIfExpn Analyze(Scope scope,
                                       NameProvider names,
                                       TvProvider tvs,
                                       PsIfExpn psIfExpn)
        {
            var condition = IAzFuncExpn.Analyze(scope, names, tvs, psIfExpn.Condition);
            var thenCase = IAzFuncExpn.Analyze(scope, names, tvs, psIfExpn.ThenCase);
            var elseCase = IAzFuncExpn.Analyze(scope, names, tvs, psIfExpn.ElseCase);

            return new(condition, thenCase, elseCase, tvs.Next, psIfExpn.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var csc = Condition.Constrain(provider, scope);
            var cst = ThenCase.Constrain(provider, scope);
            var cse = ElseCase.Constrain(provider, scope);

            var cif = new Constraint(Type, ThenCase.Type, Position);
            var cc = new Constraint(Condition.Type, Core.Instance.Bool.ToCtor(), Position);
            var cf = new Constraint(ThenCase.Type, ElseCase.Type, Position);

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

        public IAzFuncExpn SubstituteType(Substitution s)
        {
            return new AzIfExpn(Condition.SubstituteType(s),
                                ThenCase.SubstituteType(s),
                                ElseCase.SubstituteType(s),
                                Type.Substitute(s),
                                Position);
        }

        public override string ToString() => Print(0);
    }
}
