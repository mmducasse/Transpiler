using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Generate
{
    public record GnTupleExpn(IReadOnlyList<IGnFuncExpn> Elements) : IGnFuncExpn
    {
        public string Generate(int i, NameProvider names, ref string s)
        {
            throw new System.NotImplementedException();
        }
    }
}
