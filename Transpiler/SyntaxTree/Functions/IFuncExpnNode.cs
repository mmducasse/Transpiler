using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler
{
    public interface IFuncExpnNode : IFuncNode
    {
        public static bool Parse(ref TokenQueue queue, out IFuncExpnNode node)
        {
            node = null;
            var q = queue;

            if (LambdaNode.Parse(ref q, out var lambdaNode)) { node = lambdaNode; }
            else if (IfNode.Parse(ref q, out var ifNode)) { node = ifNode; }
            else if (MatchNode.Parse(ref q, out var matchNode)) { node = matchNode; }
            else if (ArbitraryExpnNode.Parse(ref q, out var arbNode)) { node = arbNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }

        public static bool ParseInline(ref TokenQueue queue, out IFuncExpnNode node)
        {
            node = null;
            var q = queue;

            if (LambdaNode.Parse(ref q, out var lambdaNode)) { node = lambdaNode; }
            else if (ArbitraryExpnNode.Parse(ref q, out var arbNode)) { node = arbNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }

        public static bool ParseMultiline(ref TokenQueue queue, out IFuncExpnNode node)
        {
            node = null;
            var q = queue;

            if (IfNode.Parse(ref q, out var ifNode)) { node = ifNode; }
            else if (MatchNode.Parse(ref q, out var matchNode)) { node = matchNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }

        public static IFuncExpnNode Analyze(Scope scope,
                                            IFuncExpnNode node)
        {
            return node switch
            {
                IfNode ifExpn => IfNode.Analyze(scope, ifExpn),
                LambdaNode lamExpn => LambdaNode.Analyze(scope, lamExpn),
                ArbitraryExpnNode arbExpn => ArbitraryExpnNode.Analyze(scope, arbExpn),
                SymbolNode symbol => SymbolNode.Analyze(scope, symbol),
                ILiteralNode literal => literal,
                _ => throw new NotImplementedException()
            };
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              IFuncExpnNode node)
        {
            tvTable.AddNode(scope, node);

            return node switch
            {
                ILiteralNode _ => ConstraintSet.Empty,
                SymbolNode symExpn => SymbolNode.Constrain(tvTable, scope, symExpn),
                AppNode appExpn => AppNode.Constrain(tvTable, scope, appExpn),
                LambdaNode lamExpn => LambdaNode.Constrain(tvTable, scope, lamExpn),
                IfNode ifExpn => IfNode.Constrain(tvTable, scope, ifExpn),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
