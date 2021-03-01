using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Generate
{
    public interface IGnTypeParameter { }

    public record GnPolyTypeParameter : IGnTypeParameter
    {
    }

    public record GnDataTypeParameter : IGnTypeParameter
    {

    }
}
