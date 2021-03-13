// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

namespace Transpiler.Parse
{
    /// <summary>
    /// An expression within a function.
    /// </summary>
    public interface IPsFuncExpn : IPsFuncNode, IPsFuncStmt
    {
        public static bool Parse(ref TokenQueue queue, bool isInline, out IPsFuncExpn node)
        {
            node = null;
            var q = queue;

            if (PsIfExpn.Parse(ref q, isInline, out var ifNode)) { node = ifNode; }
            else if (!isInline && PsMatchExpn.Parse(ref q, out var matchNode)) { node = matchNode; }
            else if (!isInline && PsClosureExpn.Parse(ref q, out var closeNode)) { node = closeNode; }
            else if (PsLambdaExpn.Parse(ref q, isInline, out var lambdaNode)) { node = lambdaNode; }
            else if (PsTupleExpn.Parse(ref q, out var tupleNode)) { node = tupleNode; }
            else if (PsArbExpn.Parse(ref q, isInline, out var arbNode)) { node = arbNode; }
            else { return false; }

            queue = q;
            return true;
        }
    }
}
