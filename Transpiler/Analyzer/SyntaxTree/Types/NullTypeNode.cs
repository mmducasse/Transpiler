using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public class NullTypeNode : ITypeExpnNode
    {
        public string Print(int indent)
        {
            return "()";
        }
    }
}
