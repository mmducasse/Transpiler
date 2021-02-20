using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsDectorPattern(string TypeName,
                                  IReadOnlyList<IPsPattern> Variables,
                                  CodePosition Position) : IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out PsDectorPattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds(TokenType.Uppercase, ref q, out string ctor)) { return false; }

            List<IPsPattern> variables = new();
            while (ParseSubPattern(ref q, out var patternNode))
            {
                variables.Add(patternNode);
            }

            node = new PsDectorPattern(ctor, variables, p);
            queue = q;

            return true;
        }

        public static bool ParseSubPattern(ref TokenQueue queue,
                                            out IPsPattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            bool inParens = false;
            if (Finds("(", ref q))
            {
                inParens = true;
            }

            if (inParens && PsTuplePattern.Parse(ref q, out var tupNode)) { node = tupNode; }
            else if (inParens && PsDectorPattern.Parse(ref q, out var dctorNode)) { node = dctorNode; }
            else if (PsAnyPattern.Parse(ref q, out var elseNode)) { node = elseNode; }
            else if (PsParam.Parse(ref q, out var parNode)) { node = parNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litExpnNode)) { node = litExpnNode; }
            else if (Finds(TokenType.Uppercase, ref q, out string nullaryCtorName))
            {
                node = new PsDectorPattern(nullaryCtorName, new List<IPsPattern>(), p);
            }

            if (node != null)
            {
                if (inParens)
                {
                    Expects(")", ref q);
                }
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i)
        {
            var vs = Variables.Select(v => v.Print(i)).Separate(" ");
            return string.Format("{0} {1}", TypeName, vs);
        }

        public override string ToString() => Print(0);
    }
}
