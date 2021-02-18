using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public record AzNewDataExpn(AzDataTypeDefn Definition) : IAzFuncExpn
    {
        public IReadOnlyList<AzSymbolExpn> Arguments { get; set; }

        public CodePosition Position => CodePosition.Null;

        public string Print(int i)
        {
            return string.Format("NEW {0}", Definition.Name);
        }

        public override string ToString() => Print(0);
    }
}
