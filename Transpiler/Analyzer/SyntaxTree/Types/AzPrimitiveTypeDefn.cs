using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Transpiler.UI;

namespace Transpiler.Analysis
{
    public record AzPrimitiveTypeDefn(string Name) : IAzDataTypeDefn
    {
        public CodePosition Position => CodePosition.Null;

        public IReadOnlyList<TypeVariable> Parameters => new List<TypeVariable>();

        public static AzPrimitiveTypeDefn Make(Scope scope, string name)
        {
            var typeDefn = new AzPrimitiveTypeDefn(name);
            scope.AddType(typeDefn);
            return typeDefn;
        }

        public string Print(int i) => Name;

        public void PrintSignature()
        {
            PrLn(Name);
        }

        public override string ToString() => Name;
    }
}
