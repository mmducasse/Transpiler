// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// A sequence of type expressions whose application order will
    /// be determined in the analyzer.
    /// </summary>
    public record PsTypeArbExpn(string TypeName,
                                IReadOnlyList<IPsTypeExpn> Children,
                                CodePosition Position) : IPsTypeExpn
    {
        public static bool Parse(ref TokenQueue queue, out IPsTypeExpn node)
        {
            node = null;
            var q = queue;

            if (ParseArb(ref q, out var arbNode)) { node = arbNode; }
            else if (IPsTypeSymbolExpn.Parse(ref q, out var symNode)) { node = symNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }

        private static bool ParseArb(ref TokenQueue queue, out IPsTypeExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            bool doContinue = true;

            if (Finds("(", ref q))
            {
                if (!IPsTypeExpn.Parse(ref q, out node))
                {
                    throw Error("Expected inline expression after '('", q);
                }
                Expects(")", ref q);
                queue = q;
                return true;
            }

            if (!Finds(TokenType.Uppercase, ref q, out string typeName)) { return false; }

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
                else if (IPsTypeSymbolExpn.Parse(ref q2, out var typeSym))
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
                node = new PsTypeArbExpn(typeName, subExpns, p);
                queue = q;
                return true;
            }

            return false;
        }

        public string Print(int i)
        {
            var children = Children.Select(c => c.Print(i)).Separate(" ", prepend: " ");
            return string.Format("({0}{1})", TypeName, children);
        }

        public override string ToString() => Print(0);
    }
}
