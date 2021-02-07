using System.Collections.Generic;

namespace Transpiler
{
    public record ClassInstance(IClassType Class,
                                INamedType Implementor,
                                IReadOnlyList<IFuncDefnNode> Functions) : IType
    {
        public bool IsSolved => true;

        public string Print(bool terse = true)
        {
            string s = string.Format("impl {0} {1}", Class.Print(), Implementor.Print());
            if (!terse)
            {
                s += "::\n";
            }

            return s;
        }
    }
}
