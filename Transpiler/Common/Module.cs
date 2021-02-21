using System.Collections.Generic;
using System.IO;
using Transpiler.Analysis;
using Transpiler.Parse;

namespace Transpiler
{
    public class Module
    {
        public string Code { get; }
        public string Name { get; set; }

        public IReadOnlyList<LexerToken> Tokens { get; set; }
        public List<Module> Dependencies { get; } = new List<Module>();

        public ParseResult ParseResult { get; set; }

        public Scope Scope { get; set; }

        public string Output { get; set; }

        public bool IsFinished { get; set; }

        public Module(string code, string name)
        {
            Code = code;
            Name = name;
        }

        public static Module Create(string filePath)
        {
            string code = File.ReadAllText(filePath);

            return new Module(code, "");
        }
    }
}
