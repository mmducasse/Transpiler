﻿using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzMatchExpn(IAzFuncExpn Argument,
                              IReadOnlyList<AzMatchCase> Cases,
                              CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public static IAzFuncExpn Analyze(Scope parentScope,
                                          PsMatchExpn node)
        {

            List<AzMatchCase> cases = new();
            foreach (var c in node.Cases)
            {
                var scope = new Scope(parentScope, "<case>");
                var matchCase = AzMatchCase.Analyze(scope, c);
                cases.Add(matchCase);
            }

            if (node.IsTerse)
            {
                var param = new AzParam("$0", node.Position);
                var symbol = new AzSymbolExpn(param, node.Position);
                var matchExpn = new AzMatchExpn(symbol, cases, node.Position);
                var scopedExpn = new AzScopedFuncExpn(matchExpn, RList<AzFuncDefn>(), parentScope, node.Position);
                return new AzLambdaExpn(param, scopedExpn, node.Position);
            }
            else
            {
                var arg = IAzFuncExpn.Analyze(parentScope, node.Argument);

                return new AzMatchExpn(arg, cases, node.Position);
            }
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

                var cinput = new Constraint(targ, tcase.Input, Position);
                var coutput = new Constraint(tmatch, tcase.Output, Position);

                cs = IConstraintSet.Union(cs, c, cinput, coutput);
            }

            return IConstraintSet.Union(csa, cs);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var caseNodes = Cases.SelectMany(c => c.GetSubnodes()).ToList();
            return this.ToArr().Concat(Argument.GetSubnodes())
                               .Concat(caseNodes).ToList();
        }

        public void PostAnalyze()
        {
            int defaultCases = Cases.Where(c => c.Pattern is AzElsePattern).Count();
            if (defaultCases == 1) { return; }

            if (defaultCases > 1)
            {
                throw Analyzer.Error("Multiple default cases specified.", Position);
            }

            // No default case specified, make sure the match is exhaustive.
            if (Argument.Type is AzTypeCtorExpn typeCtorExpn &&
                typeCtorExpn.TypeDefn is AzUnionTypeDefn unionType)
            {
                var dataTypes = unionType.Subtypes.ToList();

                foreach (var @case in Cases)
                {
                    if (@case.Pattern is AzDectorPattern dectorPattern &&
                        dectorPattern.IsCompleteMember)
                    {
                        var dectorType = dectorPattern.TypeDefn;

                        dataTypes.Remove(dectorType);
                    }
                }

                if (dataTypes.Count > 0)
                {
                    throw Analyzer.Error("Non exhaustive match expressions must contain a default case.", Position);
                }
            }
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
