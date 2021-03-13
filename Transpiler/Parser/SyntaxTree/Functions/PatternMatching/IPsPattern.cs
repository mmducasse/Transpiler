// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

namespace Transpiler.Parse
{
    /// <summary>
    /// A pattern that may be matched against in a match expression.
    /// </summary>
    public interface IPsPattern : IPsFuncNode
    {
        public static bool Parse(ref TokenQueue queue,
                                 out IPsPattern node)
        {
            node = null;
            var q = queue;

            if (PsTuplePattern.Parse(ref q, out var tupNode)) { node = tupNode; }
            else if (PsDectorPattern.Parse(ref q, out var dctorNode)) { node = dctorNode; }
            else if (PsAnyPattern.Parse(ref q, out var elseNode)) { node = elseNode; }
            else if (IPsLiteralExpn.Parse(ref q, out var litExpnNode)) { node = litExpnNode; }
            else if (PsArbExpn.Parse(ref q, isInline: true, out var arbExpnNode)) { node = arbExpnNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
