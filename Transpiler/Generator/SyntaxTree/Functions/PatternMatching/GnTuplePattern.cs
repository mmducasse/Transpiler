using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnTuplePattern(IReadOnlyList<IGnPattern> Elements) : IGnDectorPattern
    {
        public static GnTuplePattern Prepare(IScope scope, AzTuplePattern pattern)
        {
            var elements = pattern.Elements.Select(v => IGnPattern.Prepare(scope, v)).ToList();
            return new(elements);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            string GenerateVar(IGnPattern variable)
            {
                return variable switch
                {
                    GnSymbolExpn => "null",
                    GnParam => "null",
                    _ => variable.Generate(),
                };
            }

            var elements = Elements.Select(GenerateVar).Separate(", ", prepend: ", ");
            string s = string.Format("[\"\"{0}]", elements);
            return s;
        }

        public void GenerateAccessors(int i, string arg, NameProvider names, ref string s)
        {
            for (int idx = 0; idx < Elements.Count; idx++)
            {
                if (Elements[idx] is GnParam)
                {
                    string result = Elements[idx].Generate();
                    s += string.Format("{0}{1} = Get({2}, {3})\n", Indent(i), result, idx, arg);
                }
                if (Elements[idx] is IGnDectorPattern dectorPattern)
                {
                    string name = names.Next;
                    s += string.Format("{0}{1} = Get({2}, {3})\n", Indent(i), name, idx, arg);
                    dectorPattern.GenerateAccessors(i, name, names, ref s);
                }
            }
        }
    }
}
