using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record TypeSymbolNode(string Name) : ITypeExpnNode, IFuncExpnNode, IUnionTypeNodeMember
    {
        public static bool Parse(ref TokenQueue queue, out TypeSymbolNode node)
        {
            node = null;
            var q = queue;

            if (!Finds(TokenType.Alphabetic, ref q, out string name)) { return false; }

            //List<string> args = new();
            //while (Finds(TokenType.Alphabetic, ref q, out string argument))
            //{
            //    args.Add(argument);
            //}

            node = new(name);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            return string.Format("{0}", Name);
        }
    }
}
