using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsTypeArbExpn(IReadOnlyList<IPsTypeExpn> Children,
                                CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, out PsTypeArbExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            bool doContinue = true;

            List<IPsTypeExpn> subExpns = new();
            while (doContinue)
            {
                var q2 = q;
                if (Finds("(", ref q2))
                {
                    if (!IPsTypeExpn.Parse(ref q2, out var arbSubNode))
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
                else if (IPsTypeSymbol.Parse(ref q2, out var typeSym))
                {
                    subExpns.Add(typeSym);
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
                node = new PsTypeArbExpn(subExpns, p);
                queue = q;
                return true;
            }

            return false;
        }

        //public static bool ParseSimple(ref TokenQueue queue, out IPsTypeExpn node)
        //{
        //    node = null;
        //    var q = queue;

        //    if (IPsTypeSymbol.Parse(ref q, out var typeSym)) { node = typeSym; }

        //    if (node != null)
        //    {
        //        queue = q;
        //        return true;
        //    }

        //    return false;
        //}

        public string Print(int i)
        {
            var children = Children.Select(c => c.Print(i)).Separate(" ");
            return string.Format("({0})", children);
        }
    }
}
