// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

namespace Transpiler.Parse
{
    /// <summary>
    /// Definition of a function or symbol.
    /// </summary>
    public interface IPsFuncDefn : IPsDefn
    {
    }

    /// <summary>
    /// Definition of a function.
    /// </summary>
    public interface IPsFuncStmtDefn : IPsFuncDefn, IPsFuncStmt
    {
        IPsFuncExpn Expression { get; }

        public static bool Parse(ref TokenQueue queue, out IPsFuncStmtDefn node)
        {
            node = null;
            var q = queue;

            if (PsDectorFuncDefn.Parse(ref q, out var dectorDefn)) { node = dectorDefn; }
            else if (PsFuncDefn.ParseDefn(ref q, out var funcDefn)) { node = funcDefn; }
            else { return false; }

            queue = q;
            return true;
        }
    }
}
