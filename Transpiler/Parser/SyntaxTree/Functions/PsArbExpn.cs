﻿// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;
using System.Linq;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Keywords;

namespace Transpiler.Parse
{
    /// <summary>
    /// A sequence of inline symbols whose application order will
    /// be determined in the analyzer.
    /// </summary>
    public record PsArbExpn(IReadOnlyList<IPsFuncExpn> Children,
                            CodePosition Position) : IPsFuncExpn, IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, bool isInline, out PsArbExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            bool doContinue = true;

            List<IPsFuncExpn> subExpns = new();
            while (doContinue)
            {
                var q2 = q;
                if (Finds("=", ref q))
                {
                    return false;
                }
                else if (Finds("(", ref q2))
                {
                    if (IPsFuncExpn.Parse(ref q2, isInline: true, out var arbSubNode))
                    {
                        subExpns.Add(arbSubNode);
                    }
                    else
                    {
                        subExpns.Add(new PsTupleExpn(new List<IPsFuncExpn>(), q2.Position));
                    }
                    Expects(")", ref q2);
                }
                else if (Finds(")", ref q2))
                {
                    doContinue = false;
                    break;
                }
                else if (ReservedWords.Contains(q2.Current.Value))
                {
                    doContinue = false;
                    break;

                }
                else if (ParseSimple(ref q2, out var simpleNode))
                {
                    subExpns.Add(simpleNode);
                }
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
