using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzDectorPattern(AzDataTypeDefn TypeDefn,
                                  IReadOnlyList<IAzPattern> Variables,
                                  CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public bool IsCompleteMember => (Variables.Count == 0) || Variables.All(v => v is AzParam);

        private Substitution UniqueSubstitution { get; init; }

        public static AzDectorPattern Analyze(Scope scope,
                                              NameProvider names,
                                              PsDectorPattern psDectorPat)
        {
            if (!scope.TryGetNamedType(psDectorPat.TypeName, out var typeDefn))
            {
                throw Analyzer.Error("Type " + psDectorPat.TypeName + " is undefined.", psDectorPat.Position);
            }

            if (typeDefn is not AzDataTypeDefn dataTypeDefn)
            {
                throw Analyzer.Error("Type " + typeDefn.Name + " is not a deconstructable type.", psDectorPat.Position);
            }

            Substitution uniqueSubstitution;
            IAzTypeExpn type;
            if (dataTypeDefn.ParentUnion != null)
            {
                uniqueSubstitution = dataTypeDefn.ParentUnion.ToCtor().UniqueTvSubstitution(TypeVariables.Provider);
                type = dataTypeDefn.ParentUnion.ToCtor().Substitute(uniqueSubstitution);
            }
            else
            {
                uniqueSubstitution = dataTypeDefn.ToCtor().UniqueTvSubstitution(TypeVariables.Provider);
                type = dataTypeDefn.ToCtor().Substitute(uniqueSubstitution);
            }

            int numElements = dataTypeDefn.Expression.Elements.Count;

            var vars = psDectorPat.Variables.Select(v => IAzPattern.Analyze(scope, names, v)).ToList();
            if (vars.Count != numElements)
            {
                throw Analyzer.Error(string.Format("Type {0} has {1} members.", psDectorPat.TypeName, numElements), psDectorPat.Position);
            }

            return new(dataTypeDefn, vars, psDectorPat.Position)
            {
                Type = type,
                UniqueSubstitution = uniqueSubstitution
            };
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var cs = new ConstraintSet();
            var typeExpn = TypeDefn.Expression.Substitute(UniqueSubstitution);
            for (int i = 0; i < Variables.Count; i++)
            {
                var csv = Variables[i].Constrain(provider, scope);
                var c = new Constraint(typeExpn.Elements[i], Variables[i].Type, Position);

                cs = IConstraintSet.Union(cs, c, csv);
            }

            return cs;
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Variables.Foreach(v => v.Recurse(action));
            action(this);
        }

        public string Print(int i)
        {
            var vs = Variables.Select(v => v.Print(i)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}", TypeDefn.Name, vs);
        }

        public override string ToString() => Print(0);
    }
}
