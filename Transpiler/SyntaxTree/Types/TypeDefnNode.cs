using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record TypeDefnNode(string Name,
                               //IReadOnlyList<string> TypeParameters,
                               ITypeExpnNode Expression) : IDefnNode, IUnionTypeNodeMember
    {
        public static bool Parse(ref TokenQueue queue, out TypeDefnNode node)
        {
            node = null;
            var q = queue;

            if (!Finds(TokenType.Uppercase, ref q, out string name)) { return false; }
            if (Finds(TokenType.NewLine, ref q))
            {
                return false;
            }

            //List<string> args = new();
            //while (Finds(TokenType.Lowercase, ref q, out string arg))
            //{
            //    args.Add(arg);
            //}
            Expects("=", ref q);

            if (!ITypeExpnNode.Parse(ref q, out var expnNode))
            {
                throw Error("Expected type expression after '=' in type definition.", q);
            }

            node = new(name, expnNode);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            //string args = TypeParameters.Separate(" ");
            return string.Format("{0} = {1}", Name, Expression.Print(i));
        }
    }
}
