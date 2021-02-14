using System.Collections.Generic;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /*

    instance Eq Point {
	    p1 (==) p2 = ((a p1) == (a p2)) and ((b p1) == (b p2))
	    p1 (!=) p2 = not (p1 == p2)

    */
    public record PsClassInstDefn(string ClassName,
                              IPsTypeExpn Implementor,
                              IReadOnlyList<PsFuncDefn> Functions,
                              CodePosition Position) : IPsDefn
    {
        public static bool Parse(ref TokenQueue queue, out PsClassInstDefn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            int i = q.Indent;

            if (!Finds("instance", ref q)) { return false; }
            Expects(TokenType.Uppercase, ref q, out string name);
            
            if (!PsTypeArbExpn.Parse(ref q, out var implementor))
            {
                throw Error("Expected expression for class implementor in instance definition.", q);
            }

            Expects("{", ref q);
            SkipNewlines(ref q);

            var q2 = q;

            if (FindsIndents(ref q2, i + 1) &&
                !PsFuncDefn.ParseDefn(ref q2, out _))
            {
                throw Error("Expected at least 1 function definition inside type class definition.", q);
            }

            var funcDecls = new List<PsFuncDefn>();
            while (FindsIndents(ref q, i + 1) &&
                   PsFuncDefn.ParseDefn(ref q, out var funcDefn))
            {
                SkipNewlines(ref q);
                funcDecls.Add(funcDefn);
            }

            node = new PsClassInstDefn(name, implementor, funcDecls, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string s = string.Format("instance {0} {1} {{\n", ClassName, Implementor.Print(i));
            foreach (var fn in Functions)
            {
                s += string.Format("{0}{1}\n", Indent(i + 1), fn.Print(i + 1));
            }

            return s;
        }

        public override string ToString() => Print(0);
    }
}
