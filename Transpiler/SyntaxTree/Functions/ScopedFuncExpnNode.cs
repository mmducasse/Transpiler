using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parser;
using static Transpiler.Extensions;

namespace Transpiler
{
    public record ScopedFuncExpnNode(IFuncExpnNode Expression,
                                     IReadOnlyList<FuncDefnNode> SubDefinitions,
                                     Scope scope = null) : IFuncExpnNode
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

            var newSubDefns = Analyzer.AnalyzeFunctions(scope, scopedExpn.SubDefinitions);

            var newExpn = IFuncExpnNode.Analyze(scope, scopedExpn.Expression);
            scope.TvTable.AddNode(scope, newExpn);

            return new(newExpn, newSubDefns, scope);
        }

        public static bool Solve(ScopedFuncExpnNode scopedExpn)
        {
            bool p = Analyzer.SolveFunctions(scopedExpn.scope, scopedExpn.SubDefinitions);
            p |= IFuncExpnNode.Solve(scopedExpn.scope, scopedExpn.Expression);

            return p;
        }

        public string Print(int i)
        {
            string s = Expression.Print(i);
            foreach (var subDefn in SubDefinitions)
            {
                s += string.Format("\n{0}{1}", Indent(i + 1), subDefn.Print(i + 1));
            }

            return s;
        }
    }
}
