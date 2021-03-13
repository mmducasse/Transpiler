// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;
using static Transpiler.UI;

namespace Transpiler.Parse
{
    public record PsImportNode(string ModuleName,
                               CodePosition Position);

    public class ParseResult
    {
        public string ModuleName { get; set; } = null;
        public List<PsImportNode> ImportedModules { get; } = new();
        public List<IPsFuncStmtDefn> FuncDefns { get; } = new();
        public List<IPsTypeDefn> TypeDefns { get; } = new();
        public List<PsClassTypeDefn> ClassDefns { get; } = new();
        public List<PsClassInstDefn> InstDefns { get; } = new();

        public ParseResult()
        {

        }

        public void Print()
        {
            PrLn("module {0}", ModuleName);

            foreach (var import in ImportedModules)
            {
                PrLn("use {0}", import.ModuleName);
            }

            foreach (var @class in ClassDefns)
            {
                PrLn(@class.Print(0));
            }

            foreach (var inst in InstDefns)
            {
                PrLn(inst.Print(0));
            }

            foreach (var type in TypeDefns)
            {
                PrLn(type.Print(0));
            }

            foreach (var func in FuncDefns)
            {
                PrLn(func.Print(0));
            }
        }
    }
}
