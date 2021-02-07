using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public record FuncSignature(string Name, IType Type) : IFuncDefnNode
    {
        public eFixity Fixity => eFixity.Prefix;

        public string Print(int i)
        {
            return string.Format("{0} :: {1}", Name, Type.Print(terse: true));
        }
    }
}
