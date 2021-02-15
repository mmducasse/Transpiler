using System;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzAppExpn(IAzFuncExpn Function,
                            IAzFuncExpn Argument,
                            CodePosition Position) : IAzFuncExpn
    {
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

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzAppExpn node)
        {
            var csf = IAzFuncExpn.Constrain(tvTable, scope, node.Function);
            var csx = IAzFuncExpn.Constrain(tvTable, scope, node.Argument);

            var tf = tvTable.GetTypeOf(node.Function);
            var tx = tvTable.GetTypeOf(node.Argument);
            var tfx = tvTable.GetTypeOf(node);
            var ta = tvTable.TvProvider.Next;

            var p = CodePosition.Null;
            var cfx = new Constraint(tf, new AzTypeLambdaExpn(tx, tfx, p), node);

            return IConstraintSet.Union(cfx, csf, csx);
        }

        public string Print(int i)
        {
            return string.Format("({0} {1})", Function.Print(i), Argument.Print(i));
        }

        public override string ToString() => Print(0);
    }
}
