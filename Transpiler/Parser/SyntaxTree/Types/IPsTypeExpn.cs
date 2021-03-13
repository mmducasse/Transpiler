// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

namespace Transpiler.Parse
{
    /// <summary>
    /// A type expression.
    /// </summary>
    public interface IPsTypeExpn : IPsNode
    {
        public static bool Parse(ref TokenQueue queue, out IPsTypeExpn node, bool allowTuples = true)
        {
            node = null;
            var q = queue;

            if (PsTypeLambdaExpn.Parse(ref q, out var funType)) { node = funType; }
            else if (allowTuples && PsTypeTupleExpn.Parse(ref q, out var tupType)) { node = tupType; }
            else if (PsTypeArbExpn.Parse(ref q, out var arbType)) { node = arbType; }
            else { return false; }

            queue = q;
            return true;
        }
    }
}
