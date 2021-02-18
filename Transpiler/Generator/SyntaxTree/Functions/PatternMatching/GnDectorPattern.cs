using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnDectorPattern(string TypeName,
                                  IReadOnlyList<IGnPattern> Variables) : IGnPattern
    {
        public static GnDectorPattern Prepare(AzDectorPattern pattern)
        {
            var vars = pattern.Variables.Select(v => IGnPattern.Prepare(v)).ToList();
            return new(pattern.TypeDefn.Name, vars);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            string name = TypeName;
            var vars = Variables.Select(v => "null").Separate(", ", prepend: ", ");
            string s = string.Format("[\"{0}\"{1}]", name, vars);
            return s;
        }

        public void GenerateAccessors(int i, string arg, ref string s)
        {
            for (int idx = 0; idx < Variables.Count; idx++)
            {
                s += string.Format("{0}{1} = Get({2}, {3})\n", Indent(i), Variables[idx].Generate(), arg, idx);
            }
        }
    }
}
