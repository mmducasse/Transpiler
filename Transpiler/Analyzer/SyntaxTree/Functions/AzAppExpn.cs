using System;
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
                                          PsArbExpn node)
        {
            var subexpns = node.Children.Select(c => IAzFuncExpn.Analyze(scope, c)).ToList();

            if ((subexpns.Count == 3) &&
                subexpns[1] is AzSymbolExpn symExpn &&
                symExpn.Definition.Fixity == eFixity.Infix)
            {
                var op = subexpns[1];
                var app = new AzAppExpn(op, subexpns[0], op.Position);
                app = new AzAppExpn(app, subexpns[2], op.Position);
                return app;
            }
            else
            {
                IAzFuncExpn app = subexpns[0];
                var p = app.Position;
                for (int i = 1; i < subexpns.Count; i++)
                {
                    app = new AzAppExpn(app, subexpns[i], p);
                }

                return app;
            }
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            var csf = Function.Constrain(provider, scope);
            var csx = Argument.Constrain(provider, scope);

            var p = CodePosition.Null;
            var lamType = new AzTypeLambdaExpn(Argument.Type, Type, p);
            var cfx = new Constraint(Function.Type, lamType, this);

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
