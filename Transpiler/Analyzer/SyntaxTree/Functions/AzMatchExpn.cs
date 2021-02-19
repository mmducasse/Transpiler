using System.Collections.Generic;
using static Transpiler.Extensions;
using Transpiler.Parse;
using static Transpiler.CodePosition;
using System.Linq;

namespace Transpiler.Analysis
{
    public record AzMatchExpn(IAzFuncExpn Argument,
                              IReadOnlyList<AzMatchCase> Cases,
                              CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public static AzMatchExpn Analyze(Scope parentScope,
                                          PsMatchExpn node)
        {
            var arg = IAzFuncExpn.Analyze(parentScope, node.Argument);

            List<AzMatchCase> cases = new();
            foreach (var c in node.Cases)
            {
                var scope = new Scope(parentScope, "<case>");
                var matchCase = AzMatchCase.Analyze(scope, c);
                cases.Add(matchCase);
            }

            return new(arg, cases, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            var csa = Argument.Constrain(provider, scope);

            var tmatch = Type;
            var targ = Argument.Type;

            var cs = new ConstraintSet();
            foreach (var @case in Cases)
            {
                var c = @case.Constrain(provider, scope);

                var tcase = @case.Type as AzTypeLambdaExpn;

                var cinput = new Constraint(targ, tcase.Input, this);
                var coutput = new Constraint(tmatch, tcase.Output, this);

                cs = IConstraintSet.Union(cs, c, cinput, coutput);
            }

            return IConstraintSet.Union(csa, cs);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var caseNodes = Cases.SelectMany(c => c.GetSubnodes()).ToList();
            return this.ToArr().Concat(caseNodes).ToList();
        }

        public string Print(int i)
        {
            string s = string.Format("match {0}\n", Argument.Print(i));
            foreach (var c in Cases)
            {
                s += string.Format("{0}| {1}\n", Indent(i + 1), c.Print(i + 1));
            }

            return s;
        }
    }
}
