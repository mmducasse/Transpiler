using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Analysis
{
    public class AnalyzerData
    {
        public List<INamedType> Types { get; } = new();
    }
}
