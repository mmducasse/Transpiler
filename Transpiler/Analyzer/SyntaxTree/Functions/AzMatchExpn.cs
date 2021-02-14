using System.Collections.Generic;
using static Transpiler.Extensions;
using Transpiler.Parse;
using System;

namespace Transpiler.Analysis
{
    public record AzMatchExpn(IAzFuncExpn Argument,
                              IReadOnlyList<AzMatchCase> Cases,
                              CodePosition Position) : IAzFuncExpn
    {
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

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzMatchExpn node)
        {
            var csa = IAzFuncExpn.Constrain(tvTable, scope, node.Argument);

            var tmatch = tvTable.GetTypeOf(node);
            var targ = tvTable.GetTypeOf(node.Argument);

            var cs = new ConstraintSet();
            foreach (var @case in node.Cases)
            {
                tvTable.AddNode(scope, @case);
                var c = AzMatchCase.Constrain(tvTable, scope, @case);

                var tcase = tvTable.GetTypeOf(@case) as AzTypeLambdaExpn;

                var cinput = new EqualConstraint(targ, tcase.Input, node);
                var coutput = new EqualConstraint(tmatch, tcase.Output, node);

                cs = IConstraintSet.Union(cs, c, cinput, coutput);
            }


            return IConstraintSet.Union(csa, cs);
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
