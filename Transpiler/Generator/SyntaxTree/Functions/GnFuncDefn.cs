using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    // Todo: Add optional Type constraint property.
    public record GnFuncDefn(string Name,
                             IReadOnlyList<Refinement> Refinements,
                             IGnFuncExpn Expression) : IGnFuncStmtDefn
    {
        public static GnFuncDefn Prepare(Scope scope, AzFuncDefn azFuncDefn)
        {
            var rs = azFuncDefn.Type.GetRefinements();
            rs = rs.Where(r => !scope.GetGnFuncTypeParamAlreadyExists(r)).ToList();

            foreach (var r in rs) { scope.GnAddTypeParam(r); }

            var expn = IGnFuncExpn.Prepare(scope, azFuncDefn.Expression);

            return new(azFuncDefn.Name, rs, expn);
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            return Generate(i, names, "", ref s);
        }

        public string Generate(int i, NameProvider names, string namePrefix, ref string s)
        {
            string name = namePrefix.Generated() + Name.SafeNameGenerated();
            string rs = Refinements.Select(r => GnRefinement.Generate(r)).Separate(", ");
            s += string.Format("{0}function {1}({2}) {{\n", Indent(i), name, rs);
            string expnRes = Expression.Generate(i + 1, names, ref s);
            s += string.Format("{0}return {1}\n", Indent(i + 1), expnRes);
            s += Indent(i) + "}\n";
            //s += string.Format("{0}const {1} = {2}()\n", Indent(i), name, helperName);

            return name;
        }
    }
}
