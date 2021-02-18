using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Generate
{
    public interface IGnInlineNode
    {
        public string Generate();
    }
}
