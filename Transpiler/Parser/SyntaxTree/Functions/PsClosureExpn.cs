using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parse.KeySymbols;
using static Transpiler.Extensions;

namespace Transpiler.Parse
{
    public record PsClosureExpn(IReadOnlyList<IPsFuncStmt> Statements,
                                IPsFuncExpn ReturnExpression,
                                CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsClosureExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            int indent = q.Indent;

            if (!Finds("{", ref q)) { return false; }

            List<IPsFuncStmt> lines = new();
            var q2 = q;
            while (Finds(TokenType.NewLine, ref q2) &&
                   FindsIndents(ref q2, indent + 1))
            {
                if (IPsFuncExpn.Parse(ref q2, isInline: false, out var funcExpn)) { lines.Add(funcExpn); }
                else if (IPsFuncStmtDefn.Parse(ref q2, out var funcStmtDefn)) { lines.Add(funcStmtDefn); }
                else
                {
                    throw Error("Expected statement or expression in closure.", q2.Position);
                }
                q = q2;
            }

            if (lines.Count == 0)
            {
                throw Error("Closure must contain at least one expression.", q.Position);
            }

            if (lines.Last() is not IPsFuncExpn returnExpression)
            {
                throw Error("Last line of closure must be an expression.", q.Position);
            }
            lines.RemoveAt(lines.Count - 1);

            node = new PsClosureExpn(lines, returnExpression, p);

            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string s = "{\n";
            foreach (var stmt in Statements)
            {
                s += Indent(i + 1) + stmt.Print(i + 1) + "\n";
            }
            s += Indent(i + 1) + ReturnExpression.Print(i + 1) + "\n";
            return s;
        }

        public override string ToString() => Print(0);
    }
}
