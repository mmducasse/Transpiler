using System.Collections.Generic;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsDectorFuncDefn(IReadOnlyList<string> Elements,
                                   bool IsPrivate,
                                   IPsFuncExpn Expression,
                                   CodePosition Position) : IPsFuncStmtDefn
    {
        public IReadOnlyList<PsParam> Parameters => new List<PsParam>();

        public static bool Parse(ref TokenQueue queue, out PsDectorFuncDefn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            // Private indicator.
            bool isPrivate = false;
            if (Finds("_", ref q))
            {
                isPrivate = true;
            }

            // Elements.
            List<string> elements = new();
            if (!Finds(TokenType.Lowercase, ref q, out string firstElement)) { return false; }
            elements.Add(firstElement);
            while (Finds(",", ref q))
            {
                Expects(TokenType.Lowercase, ref q, out string nextElement);
                elements.Add(nextElement);
            }

            if (elements.Count == 1) { return false; }

            // Expression.
            IPsFuncExpn expn = null;
            if (Finds("=", ref q))
            {
                if (!PsScopedFuncExpn.Parse(ref q, out expn))
                {
                    throw Error("Expected function expression after '='", q);
                }
            }

            node = new(elements, isPrivate, expn, p);
            queue = q;

            return true;
        }

        public string PrintSignature(int i)
        {
            string accessMod = IsPrivate ? "_ " : "";
            return string.Format("{0}{1}", accessMod, Elements.Separate(", "));
        }

        public string Print(int i)
        {
            string accessMod = IsPrivate ? "_ " : "";
            string s = string.Format("{0}{1} = {0}", accessMod, Elements.Separate(", "), Expression.Print(i + 1));
            return s;
        }

        public override string ToString() => Print(0);
    }
}
