using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsDectorPattern(string TypeName,
                                  IReadOnlyList<PsParam> Variables,
                                  CodePosition Position) : IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out PsDectorPattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds(TokenType.Uppercase, ref q, out string ctor)) { return false; }

            List<PsParam> variables = new();
            while (PsParam.Parse(ref q, out var patternNode))
            {
                variables.Add(patternNode);
            }

            node = new PsDectorPattern(ctor, variables, p);
            queue = q;

            return true;
        }

        public string Print(int i)
        {
            var vs = Variables.Select(v => v.Print(i)).Separate(" ");
            return string.Format("{0} {1}", TypeName, vs);
        }

        public override string ToString() => Print(0);
    }
}
