using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Analysis
{
    public interface IAzDataTypeDefn : IAzTypeDefn
    {
        IReadOnlyList<TypeVariable> Parameters { get; }
    }

    public static class IAzDataTypeDefnUtil
    {
        public static AzTypeCtorExpn ToCtor(this IAzDataTypeDefn defn)
        {
            return new(defn, defn.Parameters, CodePosition.Null);
        }
    }
}
