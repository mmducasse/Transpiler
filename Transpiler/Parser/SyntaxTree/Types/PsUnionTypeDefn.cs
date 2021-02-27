using System.Collections.Generic;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Extensions;

namespace Transpiler.Parse
{
    public record PsUnionTypeDefn(string Name,
                                  IReadOnlyList<string> TypeParameters,
                                  IReadOnlyList<PsDataTypeDefn> Subtypes,
                                  CodePosition Position) : IPsTypeDefn
    {
        public static bool Parse(ref TokenQueue queue, out PsUnionTypeDefn node)
        {
            node = null;
            int indent = queue.Indent;
            var q = queue;
            var p = q.Position;

            if (!Finds(TokenType.Uppercase, ref q, out string name)) { return false; }
            if (Finds(TokenType.NewLine, ref q))
            {
                return false;
            }

            List<string> parameters = new();
            while (Finds(TokenType.Lowercase, ref q, out string par))
            {
                parameters.Add(par);
            }
            Expects("=", ref q);

            var q2 = q;
            if (!Finds(TokenType.NewLine, ref q2)) { return false; }
            if (!FindsIndents(ref q2, indent + 1)) { return false; }

            List<PsDataTypeDefn> subTypes = new();
            while (Finds(TokenType.NewLine, ref q) &&
                   FindsIndents(ref q, indent + 1) &&
                   Finds("|", ref q))
            {
                PsDataTypeDefn memberNode = null;

                if (PsDataTypeDefn.Parse(ref q, out var typeDefn)) { memberNode = typeDefn; }
                else if (ParseNullaryDataTypeDefn(ref q, out var nullaryDefn)) { memberNode = nullaryDefn; }
                //else if (PsTypeSymbol.Parse(ref q, out var symbolNode)) { memberNode = symbolNode; }

                if (memberNode != null)
                {
                    subTypes.Add(memberNode);
                }
                else
                {
                    throw Error("Expected union subtype definition or reference after '|'", q);
                }
            }

            if (subTypes.Count == 0) { return false; }

            node = new(name, parameters, subTypes, p);
            queue = q;
            return true;
        }

        private static bool ParseNullaryDataTypeDefn(ref TokenQueue queue, out PsDataTypeDefn node)
        {
            node = null;
            int indent = queue.Indent;
            var q = queue;
            var p = q.Position;

            if (Finds(TokenType.Uppercase, ref q, out string name))
            {
                var q2 = q;
                if (Finds(TokenType.NewLine, ref q2))
                {
                    node = new PsDataTypeDefn(name, new List<string>(), new List<PsDataTypeElement>(), p);
                    queue = q;
                    return true;
                }
            }

            return false;
        }


        public string Print(int i)
        {
            string ps = TypeParameters.Separate(" ", prepend: " ");
            string s = string.Format("{0}{1} =\n", Name, ps);
            int i1 = i + 1;

            foreach (var t in Subtypes)
            {
                s += string.Format("{0}| {1}\n", Indent(i1), t.Print(i1));
            }

            return s;
        }

        public override string ToString() => Print(0);
    }
}
