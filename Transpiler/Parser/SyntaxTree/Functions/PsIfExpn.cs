using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsIfExpn(IPsFuncExpn Condition,
                           IPsFuncExpn ThenCase,
                           IPsFuncExpn ElseCase,
                           CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, bool isInline, out PsIfExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            // If
            if (!Finds("if", ref q)) { return false; }
            if (!PsArbExpn.Parse(ref q, isInline, out var condNode))
            {
                throw Error("Expected expression as condition in If expression.", q);
            }

            // Then
            if (!Finds(TokenType.NewLine, ref q))
            {
                isInline = true;
            }
            if (!isInline) { ExpectsIndents(ref q, queue.Indent + 1); }
            Expects("then", ref q);
            IPsFuncExpn thenNode;
            if (isInline && IPsFuncExpn.Parse(ref q, isInline, out thenNode)) { }
            else if (PsScopedFuncExpn.Parse(ref q, out thenNode)) { }
            else
            {
                throw Error("Expected expression as then case in If expression.", q);
            }

            // Else
            if (!isInline)
            {
                Expects(TokenType.NewLine, ref q);
                ExpectsIndents(ref q, queue.Indent + 1);
            }
            Expects("else", ref q);
            IPsFuncExpn elseNode;
            if (isInline && IPsFuncExpn.Parse(ref q, isInline, out elseNode)) { }
            else if (PsScopedFuncExpn.Parse(ref q, out elseNode)) { }
            else
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

        public override string ToString() => Print(0);
    }
}
