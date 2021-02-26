using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public static class AzTypeRefinementGroup
    {
        public static void AnalyzeRefinements(Scope scope,
                                              PsTypeRefinementGroup group)
        {
            foreach (var r in group.Refinements)
            {
                if (scope.TryGetNamedType(r.TypeClassName, out var type) &&
                    type is AzClassTypeDefn classDefn)
                {
                    if (scope.TryGetTypeVar(r.TypeVarName, out var tv))
                    {
                        var newRefinements = tv.Refinements.Append(classDefn).ToList();
                        scope.AddTypeVar(r.TypeVarName, newRefinements);
                    }
                    else
                    {
                        throw Analyzer.Error("Uninitialized type variable.", r.Position);
                    }
                }
                else
                {
                    throw Analyzer.Error("Class " + r.TypeClassName + " is not defined.", r.Position);
                }
            }
        }
    }
}
