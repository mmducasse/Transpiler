using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parser;
using static Transpiler.Extensions;

namespace Transpiler
{
    public record ScopedFuncExpnNode(IFuncExpnNode Expression,
                                     IReadOnlyList<FuncDefnNode> FuncDefinitions,
                                     Scope Scope = null) : IFuncExpnNode
    {
        public static ScopedFuncExpnNode Make(IFuncExpnNode expression) =>
            new(expression, new List<FuncDefnNode>());

        public static bool Parse(ref TokenQueue queue, out ScopedFuncExpnNode node)
        {
            node = null;
            var q = queue;
            int indent = q.Indent;
            var subDefns = new List<FuncDefnNode>();

            if (!IFuncExpnNode.Parse(ref q, out var expn)) { return false; }

            var q2 = q;
            if (!(Finds(TokenType.NewLine, ref q2) &&
                  FindsIndents(ref q2, indent + 1)))
            {
                // Return node with no sub definitions.
                node = new ScopedFuncExpnNode(expn, subDefns);
                queue = q;
                return true;
            }

            Expects(TokenType.NewLine, ref q);
            while (FindsIndents(ref q, indent + 1))
            {
                if (!FuncDefnNode.Parse(ref q, out var funcDefnNode))
                {
                    throw Error("Expected function definition.", q);
                }

                subDefns.Add(funcDefnNode);
            }

            node = new ScopedFuncExpnNode(expn, subDefns);
            queue = q;
            return true;
        }

        public static ScopedFuncExpnNode Analyze(Scope parentScope,
                                                 ScopedFuncExpnNode scopedExpn)
        {
            var scope = new Scope(parentScope);

            var newSubDefns = Analyzer.AnalyzeFunctions(scope, scopedExpn.FuncDefinitions);

            var newExpn = IFuncExpnNode.Analyze(scope, scopedExpn.Expression);
            scope.TvTable.AddNode(scope, newExpn);

            return new(newExpn, newSubDefns, scope);
        }

        public static ConstraintSet Constrain(ScopedFuncExpnNode node)
        {
            var cs = new ConstraintSet();

            foreach (var fn in node.FuncDefinitions)
            {
                var fcs = FuncDefnNode.Constrain(node.Scope, fn);
                cs = IConstraints.Union(fcs, cs);
            }

            var cse = IFuncExpnNode.Constrain(node.Scope, node.Expression);

            return IConstraints.Union(cse, cs);
        }

        public string Print(int i)
        {
            string s = Expression.Print(i);
            foreach (var subDefn in FuncDefinitions)
            {
                s += string.Format("\n{0}{1}", Indent(i + 1), subDefn.Print(i + 1));
            }

            return s;
        }
    }
}
