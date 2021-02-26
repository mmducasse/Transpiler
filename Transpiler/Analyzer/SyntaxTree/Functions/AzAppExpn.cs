﻿using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzAppExpn(IAzFuncExpn Function,
                            IAzFuncExpn Argument,
                            CodePosition Position) : IAzFuncExpn, IAzPattern
    {
        public IAzTypeExpn Type { get; set; }

        public static IAzFuncExpn Analyze(Scope scope,
                                          NameProvider provider,
                                          PsArbExpn node)
        {
            var subexpns = node.Children.Select(c => IAzFuncExpn.Analyze(scope, provider, c)).ToList();
            return Arrange(provider, subexpns);
        }

        private static IAzFuncExpn Arrange(NameProvider provider,
                                           IReadOnlyList<IAzFuncExpn> subexpns)
        {
            // Zero subexpressions (illegal).
            if (subexpns.Count == 0)
            {
                throw new NotImplementedException();
            }

            // One subexpression.
            if (subexpns.Count == 1)
            {
                return subexpns[0];
            }

            // Two subexpressions and the first is an infix operator.
            if ((subexpns.Count == 2) &&
                subexpns[0] is AzSymbolExpn op1 &&
                op1.Definition.Fixity == eFixity.Infix)
            {
                var param = new AzParam(provider.Next, IsAutoGenerated: true, op1.Position);
                var symbol = new AzSymbolExpn(param, op1.Position);
                var innerApp = new AzAppExpn(op1, symbol, op1.Position);
                var outerApp = new AzAppExpn(innerApp, subexpns[1], op1.Position);

                return new AzLambdaExpn(param, outerApp, op1.Position);
            }

            // Three subexpressions and the second is an infix operator.
            if ((subexpns.Count == 3) &&
                subexpns[1] is AzSymbolExpn symExpn &&
                symExpn.Definition.Fixity == eFixity.Infix)
            {
                var op = subexpns[1];
                var app2 = new AzAppExpn(op, subexpns[0], op.Position);
                app2 = new AzAppExpn(app2, subexpns[2], op.Position);
                return app2;
            }

            // Multiple subexpressions.
            IAzFuncExpn app = subexpns[0];
            var p = app.Position;
            for (int i = 1; i < subexpns.Count; i++)
            {
                if (subexpns[i] is AzSymbolExpn opi &&
                    opi.Definition.Fixity != eFixity.Prefix)
                {
                    app = new AzAppExpn(subexpns[i], app, subexpns[i].Position);
                }
                else
                {
                    app = new AzAppExpn(app, subexpns[i], app.Position);
                }
            }

            return app;
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            var csf = Function.Constrain(provider, scope);
            var csx = Argument.Constrain(provider, scope);

            var p = CodePosition.Null;
            var lamType = new AzTypeLambdaExpn(Argument.Type, Type, p);
            var cfx = new Constraint(Function.Type, lamType, Position);

            return IConstraintSet.Union(cfx, csf, csx);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Function.GetSubnodes()).Concat(Argument.GetSubnodes()).ToList();
        }

        public string Print(int i)
        {
            return string.Format("({0} {1})", Function.Print(i), Argument.Print(i));
        }

        public override string ToString() => Print(0);
    }
}
