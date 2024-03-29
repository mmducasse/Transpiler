﻿// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// A case in a match expression.
    /// </summary>
    public record PsMatchCase(IPsPattern Pattern,
                              IPsFuncExpn Expression,
                              CodePosition Position) : IPsFuncNode
    {
        public static bool Parse(ref TokenQueue queue, out PsMatchCase node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!IPsPattern.Parse(ref q, out var pattNode)) { return false; }
            Expects("->", ref q);
            if (!PsScopedFuncExpn.Parse(ref q, out var expnNode))
            {
                throw Error("Expected expression after '->' in match case.", q);
            }

            node = new PsMatchCase(pattNode, expnNode, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), Expression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
