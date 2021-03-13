// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    /// <summary>
    /// A single element within a data type.
    /// </summary>
    public record PsDataTypeElement(string Name,
                                    IPsTypeExpn TypeExpression,
                                    CodePosition Position) : IPsNode
    {
        public bool HasAccessor => Name != null;

        public static bool Parse(ref TokenQueue queue, out PsDataTypeElement node)
        {
            var q = queue;
            node = null;
            var p = q.Position;

            var q2 = q;
            if (Finds(TokenType.Lowercase, ref q2, out string name) &&
                Finds("::", ref q2))
            {
                q = q2;
                if (!IPsTypeExpn.Parse(ref q, out var typeExpn, allowTuples: false))
                {
                    throw Error("Expected type expression after '::'.", q.Position);
                }

                node = new(name, typeExpn, p);
            }
            else
            {
                if (!IPsTypeExpn.Parse(ref q, out var typeExpn, allowTuples: false))
                {
                    return false;
                }

                node = new(null, typeExpn, p);
            }

            queue = q;
            return true;
        }

        public string Print(int i)
        {
            if (HasAccessor)
            {
                return string.Format("{0} :: {1}", Name, TypeExpression.Print(i));
            }
            return TypeExpression.Print(i);
        }

        public override string ToString() => Print(0);
    }

}
