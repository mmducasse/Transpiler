using System;
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
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzIfExpn Analyze(Scope scope,
                                       NameProvider names,
                                       PsIfExpn psIfExpn)
        {
            var condition = IAzFuncExpn.Analyze(scope, names, psIfExpn.Condition);
            var thenCase = IAzFuncExpn.Analyze(scope, names, psIfExpn.ThenCase);
            var elseCase = IAzFuncExpn.Analyze(scope, names, psIfExpn.ElseCase);

            return new(condition, thenCase, elseCase, psIfExpn.Position);
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

        public string Print(int i)
        {
            int i1 = i + 1;
            string s = string.Format("if {0}\n", Condition.Print(i));
            s += string.Format("{0}then {1}\n", Indent(i1), ThenCase.Print(i1));
            s += string.Format("{0}else {1}\n", Indent(i1), ElseCase.Print(i1));

            return s;
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Condition.Recurse(action);
            ThenCase.Recurse(action);
            ElseCase.Recurse(action);
            action(this);
        }

        public override string ToString() => Print(0);
    }
}
