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

        public Scope Scope { get; }

        public bool IsAnalyzed { get; set; } = false;

        public static Module CoreModule() => new Module("", "Core", new());

        public Module(string code, string name) 
            : this(code, name, new Scope(Core.Instance.Scope.ToArr()))
        { }

        private Module(string code, string name, Scope scope)
        {
            Code = code;
            Name = name;
            Scope = scope;
        }

        public static Module Create(string filePath)
        {
            string code = File.ReadAllText(filePath);

            return new Module(code, "");
        }

        public override string ToString() => Name;
    }
}
