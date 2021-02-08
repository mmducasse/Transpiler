using System;
using System.Collections.Generic;
using System.IO;
using Transpiler.Analysis;
using Transpiler.Parse;

namespace Transpiler
{
    public class Module
    {
        public string Code { get; }
        public string Name { get; }

        public IReadOnlyList<LexerToken> Tokens { get; set; }
        public List<Module> Dependencies { get; } = new List<Module>();

        public ParseResult ParseResult { get; set; }

        public Scope Scope { get; set; }

        //public IReadOnlyList<Command> Commands { get; set; }

        //public IDebugInfo DebugInfo { get; set; }

        public Module(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public static Module Create(string filePath)
        {
            string code = File.ReadAllText(filePath);
            string name = Path.GetFileNameWithoutExtension(filePath);

            return new Module(code, name);
        }
    }
}
