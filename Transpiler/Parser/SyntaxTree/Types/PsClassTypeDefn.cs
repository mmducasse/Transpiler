using System.Collections.Generic;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{

    /*
    
    type Eq a {
	    x (==) y : a -> a -> Bool = x != y
	    x (!=) y : a -> a -> Bool = x == y

    */
    public record PsClassTypeDefn(string Name,
                                  string TypeVar,
                                  IReadOnlyList<PsFuncDefn> Functions,
                                  CodePosition Position) : IPsTypeDefn
    {
        public static bool Parse(ref TokenQueue queue, out PsClassTypeDefn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            int i = q.Indent;

            if (!Finds("type", ref q)) { return false; }
            Expects(TokenType.Uppercase, ref q, out string name);
            Expects(TokenType.Lowercase, ref q, out string typeVar);
            Expects("{", ref q);
            SkipNewlines(ref q);

            var q2 = q;

            if (FindsIndents(ref q2, i + 1) &&
                !PsFuncDefn.ParseDecl(ref q2, out _))
            {
                throw Error("Expected at least 1 function definition inside type class definition.", q);
            }

            var funcDecls = new List<PsFuncDefn>();
            while (FindsIndents(ref q, i + 1) && 
                   PsFuncDefn.ParseDecl(ref q, out var funcDecl))
            {
                SkipNewlines(ref q);
                funcDecls.Add(funcDecl);
            }

            node = new PsClassTypeDefn(name, typeVar, funcDecls, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string s = string.Format("type {0} {1} {{\n", Name, TypeVar);
            foreach (var fn in Functions)
            {
                s += string.Format("{0}{1}\n", Indent(i + 1), fn.Print(i + 1));
            }

            return s;
        }

        public override string ToString() => Print(0);
    }
}
