using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public interface IPsTypeSymbol : IPsTypeExpn
    {
        string Name { get; }

        public static bool Parse(ref TokenQueue queue, out PsTypeSymbol node)
        {
            node = null;
            var q = queue;

            if (PsTypeSymbol.Parse(ref q, out var typeSym)) { node = typeSym; }
            else if (PsTypeVarSymbol.Parse(ref q, out var typeVar)) { node = typeVar; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }

    public record PsTypeSymbol(string Name,
                               CodePosition Position) : IPsTypeSymbol
    {
        public static bool Parse(ref TokenQueue queue, out PsTypeSymbol node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds(TokenType.Uppercase, ref q, out string name)) { return false; }

            node = new(name, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return string.Format("{0}", Name);
        }
    }


    public record PsTypeVarSymbol(string Name,
                                  CodePosition Position) : IPsTypeSymbol
    {
        public static bool Parse(ref TokenQueue queue, out PsTypeSymbol node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds(TokenType.Lowercase, ref q, out string name)) { return false; }

            node = new(name, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return string.Format("{0}", Name);
        }
    }
}
