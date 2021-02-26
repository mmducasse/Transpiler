using System.Collections.Generic;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Keywords;

namespace Transpiler.Parse
{
    public record PsClassInstDefn(PsTypeRefinementGroup Refinements,
                                  string ClassName,
                                  string ImplementorName,
                                  IReadOnlyList<string> TypeParameters,
                                  IReadOnlyList<PsFuncDefn> Functions,
                                  CodePosition Position) : IPsDefn
    {
        public static bool Parse(ref TokenQueue queue, out PsClassInstDefn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            int i = q.Indent;

            if (!Finds(TypeInstance, ref q)) { return false; }

            PsTypeRefinementGroup.Parse(ref q, out var refinements);

            Expects(TokenType.Uppercase, ref q, out string name);
            Expects(TokenType.Uppercase, ref q, out string implementorName);

            List<string> typeParams = new();
            while (Finds(TokenType.Lowercase, ref q, out string param))
            {
                typeParams.Add(param);
            }

            Expects("=", ref q);
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

            node = new PsClassInstDefn(refinements, name, implementorName, typeParams, funcDecls, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            var typeParams = TypeParameters.Separate(" ", prepend: " ");
            string s = string.Format("{0} {1}{2} {3}{4} =\n", TypeInstance, Refinements, ClassName, ImplementorName, typeParams);
            foreach (var fn in Functions)
            {
                s += string.Format("{0}{1}\n", Indent(i + 1), fn.Print(i + 1));
            }

            return s;
        }

        public override string ToString() => Print(0);
    }
}
