using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record DeconstructPatternNode(string Constructor,
                                         IReadOnlyList<IPatternNode> Variables) : IPatternNode
    {
        public static bool Parse(ref TokenQueue queue, out DeconstructPatternNode node)
        {
            node = null;
            var q = queue;

            if (!Finds(TokenType.Uppercase, ref q, out string ctor)) { return false; }

            List<IPatternNode> variables = new();
            while (IPatternNode.Parse(ref q, out var patternNode))
            {
                variables.Add(patternNode);
            }

            node = new DeconstructPatternNode(ctor, variables);
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
