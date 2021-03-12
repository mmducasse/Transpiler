using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnDectorPattern(string TypeName,
                                  IReadOnlyList<IGnPattern> Variables) : IGnDectorPattern
    {
        public static GnDectorPattern Prepare(IScope scope, AzDectorPattern pattern)
        {
            var vars = pattern.Variables.Select(v => IGnPattern.Prepare(scope, v)).ToList();
            return new(pattern.TypeDefn.Name, vars);
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

            string name = TypeName;
            var vars = Variables.Select(GenerateVar).Separate(", ", prepend: ", ");
            string s = string.Format("[\"{0}\"{1}]", name, vars);
            return s;
        }

        public void GenerateAccessors(int i, string arg, NameProvider names, ref string s)
        {
            for (int idx = 0; idx < Variables.Count; idx++)
            {
                if (Variables[idx] is GnParam)
                {
                    string result = Variables[idx].Generate();
                    s += string.Format("{0}let {1} = Get({2}, {3})\n", Indent(i), result, idx, arg);
                }
                if (Variables[idx] is IGnDectorPattern dectorPattern)
                {
                    string name = names.Next;
                    s += string.Format("{0}let {1} = Get({2}, {3})\n", Indent(i), name, idx, arg);
                    dectorPattern.GenerateAccessors(i, name, names, ref s);
                }
            }
        }
    }
}
