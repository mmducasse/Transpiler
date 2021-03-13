// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

namespace Transpiler.Parse
{
    /// <summary>
    /// Syntax Tree node that was produced by the parser.
    /// </summary>
    public interface IPsNode
    {
        CodePosition Position { get; }

        string Print(int i);
    }
}
