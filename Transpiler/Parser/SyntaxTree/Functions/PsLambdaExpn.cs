// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// A lambda expression.
    /// </summary>
    public record PsLambdaExpn(IPsPattern Parameter,
                               IPsFuncExpn Expression,
                               CodePosition Position) : IPsFuncExpn
    {
        public static bool Parse(ref TokenQueue queue, bool isInline, out PsLambdaExpn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!ParseLambdaInput(ref q, out var paramNode)) { return false; }
            if (!Finds("->", ref q)) { return false; }
            if (!IPsFuncExpn.Parse(ref q, isInline, out var expnNode))
            {
                throw Error("Expected expression after '->' in lambda function.", q);
            }

            node = new PsLambdaExpn(paramNode, expnNode, p);
            queue = q;
            return true;
        }

        private static bool ParseLambdaInput(ref TokenQueue queue, out IPsPattern node)
        {
            node = null;
            var q = queue;
            var p = q.Position;
            var q2 = q;

            if (PsAnyPattern.Parse(ref q, out var anyPattern)) { node = anyPattern; }
            else if (PsParam.Parse(ref q, out var param)) { node = param; }
            else if (Finds("(", ref q2) && Finds(")", ref q2))
            {
                node = new PsTuplePattern(new List<IPsPattern>(), p);
                q = q2;
            }
            else { return false; }

            queue = q;
            return true;
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Parameter.Print(indent + 1),
                                 Expression.Print(indent + 1));
        }
    }
}
