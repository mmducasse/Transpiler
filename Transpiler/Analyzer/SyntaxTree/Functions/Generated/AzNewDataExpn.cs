﻿using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public class AzNewDataExpn : IAzFuncExpn
    {
        public IReadOnlyList<AzSymbolExpn> Arguments { get; set; }

        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn Type { get; private set; }

        public AzDataTypeDefn Definition { get; }

        public AzNewDataExpn(AzDataTypeDefn definition)
        {
            Definition = definition;

            if (Definition.ParentUnion != null)
            {
                Type = Definition.ParentUnion.ToCtor().WithUniqueTvs(TypeVariables.Provider);
            }
            else
            {
                Type = Definition.ToCtor().WithUniqueTvs(TypeVariables.Provider);
            }
        }

        public ConstraintSet Constrain() => ConstraintSet.Empty;

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(System.Action<IAzFuncNode> action)
        {
            Arguments.Foreach(a => a.Recurse(action));
            action(this);
        }

        public string Print(int i)
        {
            var args = Arguments.Select(a => a.Print(0)).Separate(" ", prepend: " ");
            return string.Format("NEW {0}{1}", Definition.Name, args);
        }

        public override string ToString() => Print(0);
    }
}
