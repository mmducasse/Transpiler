using System.Collections.Generic;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsScopedFuncExpn(IPsFuncExpn Expression,
                                   IReadOnlyList<PsFuncDefn> FuncDefinitions,
                                   CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsScopedFuncExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            int indent = q.Indent;
            var subDefns = new List<PsFuncDefn>();

            if (!IPsFuncExpn.Parse(ref q, out var expn)) { return false; }

            var q2 = q;
            if (!(Finds(TokenType.NewLine, ref q2) &&
                  FindsIndents(ref q2, indent + 1)))
            {
                // Return node with no sub definitions.
                node = new PsScopedFuncExpn(expn, subDefns, p);
                queue = q;
                return true;
            }

            Expects(TokenType.NewLine, ref q);
            while (FindsIndents(ref q, indent + 1))
            {
                if (!PsFuncDefn.ParseDefn(ref q, out var funcDefnNode))
                {
                    throw Error("Expected function definition.", q);
                }

                subDefns.Add(funcDefnNode);
            }

            node = new PsScopedFuncExpn(expn, subDefns, p);
            queue = q;
            return true;
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

        public override string ToString() => Print(0);
    }
}
