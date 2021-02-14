using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsArbExpn(IReadOnlyList<IPsFuncExpn> Children,
                            CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsArbExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            bool doContinue = true;

            List<IPsFuncExpn> subExpns = new();
            while (doContinue)
            {
                var q2 = q;
                if (Finds("(", ref q2))
                {
                    if (!IPsFuncExpn.ParseInline(ref q2, out var arbSubNode))
                    {
                        throw Error("Expected inline expression after '('", q2);
                    }

                    subExpns.Add(arbSubNode);
                    Expects(")", ref q2);
                }
                else if (Finds(")", ref q2))
                {
                    doContinue = false;
                    break;
                }
                else if (ParseSimple(ref q2, out var simpleNode))
                {
                    subExpns.Add(simpleNode);
                }
                //else if (IFuncExpnNode.ParseMultiline(ref q2, out var expnNode))
                //{
                //    subExpns.Add(expnNode);
                //    doContinue = false;
                //}
                else
                {
                    doContinue = false;
                }

                q = q2;
            }

            if (subExpns.Count > 0)
            {
                node = new PsArbExpn(subExpns, p);
                queue = q;
                return true;
            }

            return false;
        }

        public static bool ParseSimple(ref TokenQueue queue, out IPsFuncExpn node)
        {
            node = null;
            var q = queue;

            if (IPsLiteralExpn.Parse(ref q, out var litNode)) { node = litNode; }
            else if (PsSymbolExpn.Parse(ref q, out var varNode)) { node = varNode; }
            // Operator?
            // Parens?

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i)
        {
            var children = Children.Select(c => c.Print(i)).Separate(" ");
            return string.Format("({0})", children);
        }

        public override string ToString() => Print(0);
    }
}
