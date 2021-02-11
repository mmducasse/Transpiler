using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsDeCtorPattern(string Constructor,
                                  IReadOnlyList<IPsPattern> Variables,
                          CodePosition Position = null) : IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out PsDeCtorPattern node)
        {
            node = null;
            var q = queue;

            if (!Finds(TokenType.Uppercase, ref q, out string ctor)) { return false; }

            List<IPsPattern> variables = new();
            while (IPsPattern.Parse(ref q, out var patternNode))
            {
                variables.Add(patternNode);
            }

            node = new PsDeCtorPattern(ctor, variables);
            queue = q;

            return true;
        }

        public string Print(int i)
        {
            var vs = Variables.Select(v => v.Print(i)).Separate(" ");
            return string.Format("{0} {1}", Constructor, vs);
        }
    }
}
