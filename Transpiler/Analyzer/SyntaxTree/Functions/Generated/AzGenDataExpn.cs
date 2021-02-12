using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Analysis
{
    public record AzGenDataExpn(AzDataTypeDefn Definition) : IAzFuncExpn
    {
        public CodePosition Position => CodePosition.Null;

        public string Print(int i)
        {
            return string.Format("NEW {0}", Definition.Name);
        }
    }
}
