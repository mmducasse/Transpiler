// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// A data type definition.
    /// </summary>
    public record PsDataTypeDefn(string Name,
                                 IReadOnlyList<string> TypeParameters,
                                 IReadOnlyList<PsDataTypeElement> Elements,
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

            var q2 = q;
            int indent = q2.Indent + 1;
            List<PsDataTypeElement> elements = new();
            if (Finds(TokenType.NewLine, ref q2))
            {
                // Multi-line definition.
                while (Finds(TokenType.NewLine, ref q) &&
                       FindsIndents(ref q, indent))
                {
                    if (!PsDataTypeElement.Parse(ref q, out var element))
                    {
                        throw Error("Expected data type element.", q);
                    }
                    elements.Add(element);
                }
                if (elements.Count == 0)
                {
                    throw Error("Expected at least one element in data type.", q);
                }
            }
            else
            {
                // Inline definition.
                if (!PsDataTypeElement.Parse(ref q, out var firstElement))
                {
                    throw Error("Expected at least one element in data type.", q);
                }
                elements.Add(firstElement);
                while (Finds(",", ref q))
                {
                    if (!PsDataTypeElement.Parse(ref q, out var nextElement))
                    {
                        throw Error("Expected element after ','.", q);
                    }
                    elements.Add(nextElement);
                }
            }

            node = new(name, parameters, elements, p);
            queue = q;
            return true;
        }

        public string Print(int i)
        {
            string ps = TypeParameters.Separate(" ", prepend: " ");
            string es = Elements.Separate(", ");
            return string.Format("{0}{1} = {2}", Name, ps, es);
        }

        public override string ToString() => Print(0);
    }
}
