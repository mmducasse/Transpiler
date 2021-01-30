using System.Collections.Generic;
using System.IO;
using Transpiler.Parse;

namespace Transpiler
{
    public class Module : IScope
    {
        public string Code { get; }
        public string Name { get; }

        public IReadOnlyList<LexerToken> Tokens { get; set; }
        public List<Module> Dependencies { get; } = new List<Module>();

        public ParseResult ParseResult { get; set; }

        public IScope Scope { get; set; }

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

        public bool TryGetType(string typeName, out INamedType type)
        {
            if (CoreTypes.Instance.TryGetType(typeName, out type))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.Scope.TryGetType(typeName, out type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetTypeForDefnName(string symbol, out IType type)
        {
            if (CoreTypes.Instance.TryGetTypeForDefnName(symbol, out type))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.Scope.TryGetTypeForDefnName(symbol, out type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool VerifySymbols(params string[] symbols)
        {
            foreach (string s in symbols)
            {
                if (!TryGetTypeForDefnName(s, out _))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
