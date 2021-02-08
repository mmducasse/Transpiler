using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsIfExpn(IPsFuncExpn Condition,
                           PsScopedFuncExpn ThenCase,
                           PsScopedFuncExpn ElseCase,
                           CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsIfExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            // If
            if (!Finds("if", ref q)) { return false; }
            if (!PsArbExpn.Parse(ref q, out var condNode))
            {
                throw Error("Expected expression as condition in If expression.", q);
            }

            // Then
            Expects(TokenType.NewLine, ref q);
            ExpectsIndents(ref q, queue.Indent + 1);
            Expects("then", ref q);
            if (!PsScopedFuncExpn.Parse(ref q, out var thenNode))
            {
                throw Error("Expected expression as then case in If expression.", q);
            }

            // Else
            Expects(TokenType.NewLine, ref q);
            ExpectsIndents(ref q, queue.Indent + 1);
            Expects("else", ref q);
            if (!PsScopedFuncExpn.Parse(ref q, out var elseNode))
            {
                throw Error("Expected expression as else case in If expression.", q);
            }

            node = new PsIfExpn(condNode, thenNode, elseNode, p);
            queue = q;
            return true;
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
