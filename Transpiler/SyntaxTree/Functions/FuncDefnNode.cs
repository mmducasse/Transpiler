using System;
using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public interface IFuncDefnNode : IDefnNode
    {
        string Name { get; }
    }

    // Todo: Add optional Type constraint property.
    public record FuncDefnNode(string Name,
                               ScopedFuncExpnNode ScopedExpression) : IFuncDefnNode
    {
        public static bool Parse(ref TokenQueue queue, out FuncDefnNode node)
        {
            node = null;
            var q = queue;

            // Name
            if (!Finds(TokenType.Lowercase, ref q, out string name)) { return false; }

            // Arguments
            Stack<string> args = new();
            while (Finds(TokenType.Lowercase, ref q, out string arg))
            {
                args.Push(arg);
            }

            // Optional Type Specification
            if (Finds(":", ref q))
            {
                // ParseTypeExpn
                throw new NotSupportedException();
            }

            Expects("=", ref q);

            if (!ScopedFuncExpnNode.Parse(ref q, out var scopedExpn))
            {
                throw Error("Expected function expression after '='", q);
            }

            // Turn args into lambdas.
            while (args.TryPop(out string arg))
            {
                var argNode = new ArgNode(arg);
                var expn = scopedExpn.Expression;
                var lambda = new LambdaNode(argNode, expn);
                scopedExpn = new ScopedFuncExpnNode(lambda, scopedExpn.FuncDefinitions);
            }

            node = new(name, scopedExpn);
            queue = q;

            return true;
        }

        public static ConstraintSet Constrain(Scope scope,
                                              FuncDefnNode node)
        {
            var table = scope.TvTable;

            var tf = table.GetTypeOf(node);
            var te = table.GetTypeOf(node.ScopedExpression.Expression);

            var c = new Constraint(tf, te, node);

            var cs = ScopedFuncExpnNode.Constrain(node.ScopedExpression);

            return IConstraints.Union(c, cs);
        }

        public string Print(int indent)
        {
            return string.Format("{0} = {1}", Name, ScopedExpression.Print(indent + 1));
        }
    }
}
