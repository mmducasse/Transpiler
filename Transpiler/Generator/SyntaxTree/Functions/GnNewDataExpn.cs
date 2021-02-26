using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public record GnNewDataExpn(string Name,
                                IReadOnlyList<string> Arguments) : IGnFuncExpn, IGnInlineNode
    {
        public static GnNewDataExpn Prepare(IScope scope, AzNewDataExpn newExpn)
        {
            var args = newExpn.Arguments.Select(a => a.Definition.Name).ToList();
            return new(newExpn.Definition.Name, args);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate();
        }

        public string Generate()
        {
            var args = Arguments.Select(a => a.SafeNameGenerated()).Separate(", ", prepend: ", ");
            string s = string.Format("[\"{0}\"{1}]", Name, args);
            return s;
        }
    }
}
