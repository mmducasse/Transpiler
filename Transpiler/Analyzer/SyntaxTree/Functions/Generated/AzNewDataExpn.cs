using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public class AzNewDataExpn : IAzFuncExpn
    {
        public IReadOnlyList<AzSymbolExpn> Arguments { get; set; }

        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn Type { get; private set; }

        public AzDataTypeDefn Definition { get; }

        public AzNewDataExpn(AzDataTypeDefn definition,
                             TvProvider tvs)
        {
            Definition = definition;

            if (Definition.ParentUnion != null)
            {
                Type = Definition.ParentUnion.ToCtor().WithUniqueTvs(tvs);
            }
            else
            {
                Type = Definition.ToCtor().WithUniqueTvs(tvs);
            }
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope) => ConstraintSet.Empty;

        public IAzFuncExpn SubstituteType(Substitution s)
        {
            Arguments = Arguments.Select(a => a.SubstituteType(s) as AzSymbolExpn).ToList();
            Type = Type.Substitute(s);
            return this;
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr();
        }

        public string Print(int i)
        {
            var args = Arguments.Select(a => a.Print(0)).Separate(" ", prepend: " ");
            return string.Format("NEW {0}{1}", Definition.Name, args);
        }

        public override string ToString() => Print(0);
    }
}
