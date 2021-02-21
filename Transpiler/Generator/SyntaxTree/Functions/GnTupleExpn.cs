using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnTupleExpn(IReadOnlyList<IGnFuncExpn> Elements) : IGnFuncExpn
    {
        public static GnTupleExpn Prepare(IScope scope, AzTupleExpn tupExpn)
        {
            var elements = tupExpn.Elements.Select(e => IGnFuncExpn.Prepare(scope, e)).ToList();

            return new(elements);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            List<string> elementVars = new();
            foreach (var e in Elements)
            {
                string v = e.Generate(i, names, ref s);
                elementVars.Add(v);
            }

            string tupArgs = elementVars.Separate(", ");
            string tupString = string.Format("[\"\", {0}]", tupArgs);
            return tupString;
        }
    }
}
