using System.Collections.Generic;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public record PsDataTypeDefn(string Name,
                                 IReadOnlyList<string> TypeParameters,
                                 IPsTypeExpn Expression,
                                 CodePosition Position) : IPsTypeDefn
    {
        public static bool Parse(ref TokenQueue queue, out PsDataTypeDefn node)
        {
            node = null;
            var q = queue;
            var p = q.Position;

            if (!Finds(TokenType.Uppercase, ref q, out string name)) { return false; }
            if (Finds(TokenType.NewLine, ref q))
            {
                return false;
            }

            List<string> parameters = new();
            while (Finds(TokenType.Lowercase, ref q, out string arg))
            {
                parameters.Add(arg);
            }
            Expects("=", ref q);

            if (!IPsTypeExpn.Parse(ref q, out var expnNode))
            {
                throw Error("Expected type expression after '=' in type definition.", q);
            }

            node = new(name, parameters, expnNode, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string ps = TypeParameters.Separate(" ", prepend: " ");
            return string.Format("{0}{1} = {2}", Name, ps, Expression.Print(i));
        }
    }
}
