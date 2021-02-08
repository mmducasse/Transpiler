using System;
using System.Collections.Generic;

namespace Transpiler.Parse
{
    public class ParseResult
    {
        public string ModuleName { get; set; } = null;
        public List<string> ImportedModules { get; } = new();
        public List<PsFuncDefn> FuncDefns { get; } = new();
        public List<PsTypeDefn> TypeDefns { get; } = new();
        public List<PsClassTypeDefn> ClassDefns { get; } = new();
        public List<PsClassInst> InstDefns { get; } = new();

        public ParseResult()
        {

        }

        public void Print()
        {
            Console.WriteLine("module {0}", ModuleName);

            foreach (var import in ImportedModules)
            {
                Console.WriteLine("use {0}", import);
            }

            List<PsTypeDefn> typeDefns = new();

            foreach (var type in TypeDefns)
            {
                //typeDefns.AddRange(Analyzer.FlattenTypeDefnNode(new Scope(), type));
            }
            foreach (var type in typeDefns)
            {
                Console.WriteLine(type.Print(0));
            }

            foreach (var func in FuncDefns)
            {
                Console.WriteLine(func.Print(0));
            }
        }
    }
}
