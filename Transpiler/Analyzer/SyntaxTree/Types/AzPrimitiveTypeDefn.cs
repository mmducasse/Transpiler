using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Analysis
{
    public record AzPrimitiveTypeDefn(string Name) : IAzTypeDefn
    {
        public CodePosition Position => CodePosition.Null;

        public static AzPrimitiveTypeDefn Make(Scope scope, string name)
        {
            var typeDefn = new AzPrimitiveTypeDefn(name);
            scope.AddType(typeDefn);
            return typeDefn;
        }

        public string Print(int i) => Name;
    }
}
