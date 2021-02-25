using System.Collections.Generic;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsMatchExpn(IPsFuncExpn Argument,
                              IReadOnlyList<PsMatchCase> Cases,
                              CodePosition Position) : IPsFuncExpn
    {
        public bool IsTerse => Argument == null;

        public static bool Parse(ref TokenQueue queue, out PsMatchExpn node)
        {
            node = null;
            int indent = queue.Indent;
            var q = queue;
            var p = q.Position;

            var q2 = q;
            if (!Finds(TokenType.NewLine, ref q2) &&
                !Finds("match", ref q2)) { return false; }

            IPsFuncExpn condition = null;
            if (Finds("match", ref q))
            {
                if (!IPsFuncExpn.Parse(ref q, isInline: true, out condition))
                {
                    throw Error("Expected inline expression after 'match'", q);
                }
            }

            var q3 = q;
            if (!Finds(TokenType.NewLine, ref q3)) { return false; }
            if (!FindsIndents(ref q3, indent + 1)) { return false; }
            if (!Finds("|", ref q3)) { return false; }

            List<PsMatchCase> cases = new();
            while (Finds(TokenType.NewLine, ref q) &&
                   FindsIndents(ref q, indent + 1) &&
                   Finds("|", ref q))
            {
                if (!PsMatchCase.Parse(ref q, out var matchCaseNode))
                {
                    throw Error("Expected pattern match case after '|'", q);
                }

                cases.Add(matchCaseNode);
            }

            node = new PsMatchExpn(condition, cases, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string s = "";
                
            if (!IsTerse)
            {
                string.Format("match {0}\n", Argument.Print(i));
            }

            foreach (var c in Cases)
            {
                s += string.Format("{0}| {1}\n", Indent(i + 1), c.Print(i + 1));
            }

            return s;
        }

        public override string ToString() => Print(0);
    }
}
