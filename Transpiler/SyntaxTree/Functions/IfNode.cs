using Transpiler.Parse;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parser;

namespace Transpiler
{
    public record IfNode(IFuncExpnNode Condition,
                         ScopedFuncExpnNode ThenCase,
                         ScopedFuncExpnNode ElseCase) : IFuncExpnNode
    {
        public static bool Parse(ref TokenQueue queue, out IfNode node)
        {
            node = null;
            var q = queue;

            // If
            if (!Finds("if", ref q)) { return false; }
            if (!ArbitraryExpnNode.Parse(ref q, out var condNode))
            {
                throw Error("Expected expression as condition in If expression.", q);
            }

            // Then
            Expects(TokenType.NewLine, ref q);
            ExpectsIndents(ref q, queue.Indent + 1);
            Expects("then", ref q);
            if (!ScopedFuncExpnNode.Parse(ref q, out var thenNode))
            {
                throw Error("Expected expression as then case in If expression.", q);
            }

            // Else
            Expects(TokenType.NewLine, ref q);
            ExpectsIndents(ref q, queue.Indent + 1);
            Expects("else", ref q);
            if (!ScopedFuncExpnNode.Parse(ref q, out var elseNode))
            {
                throw Error("Expected expression as else case in If expression.", q);
            }

            node = new IfNode(condNode, thenNode, elseNode);
            queue = q;
            return true;
        }

        public static IfNode Analyze(Scope scope,
                                         IfNode ifExpn)
        {
            var condition = IFuncExpnNode.Analyze(scope, ifExpn.Condition);
            var thenCase = ScopedFuncExpnNode.Analyze(scope, ifExpn.ThenCase);
            var elseCase = ScopedFuncExpnNode.Analyze(scope, ifExpn.ElseCase);

            var newIfExpn = new IfNode(condition, thenCase, elseCase);
            scope.TvTable.AddNode(scope, newIfExpn);
            return newIfExpn;
        }

        public string Print(int i)
        {
            int i1 = i + 1;
            string s = string.Format("if {0}\n", Condition.Print(i));
            s += string.Format("{0}then {1}\n", Indent(i1), ThenCase.Print(i1));
            s += string.Format("{0}else {1}\n", Indent(i1), ElseCase.Print(i1));

            return s;
        }
    }
}
