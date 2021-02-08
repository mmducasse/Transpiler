using System.Collections.Generic;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsMatch(IPsFuncExpn Argument,
                          IReadOnlyList<PsMatchCase> Cases,
                          CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsMatch node)
        {
            node = null;
            int indent = queue.Indent;
            var q = queue;
            var p = q.Position;

            if (!Finds("match", ref q)) { return false; }
            if (!PsArbExpn.Parse(ref q, out var conditionNode))
            {
                throw Error("Expected inline expression after 'match'", q);
            }

            var q2 = q;
            if (!Finds(TokenType.NewLine, ref q2)) { return false; }
            if (!FindsIndents(ref q2, indent + 1)) { return false; }
            if (!Finds("|", ref q2)) { return false; }

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

            node = new PsMatch(conditionNode, cases, p);
            queue = q;
            return true;
        }


        public string Print(int i)
        {
            string s = string.Format("match {0}\n", Argument.Print(i));
            foreach (var c in Cases)
            {
                s += string.Format("{0}| {1}\n", Indent(i + 1), c.Print(i + 1));
            }

            return s;
        }
    }
}
