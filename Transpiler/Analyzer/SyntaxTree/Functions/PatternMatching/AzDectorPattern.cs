using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzDectorPattern(AzDataTypeDefn TypeDefn,
                                  IReadOnlyList<IAzPattern> Variables,
                                  CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; set; }

        public bool IsCompleteMember => (Variables.Count == 0) || Variables.All(v => v is AzParam);

        public static AzDectorPattern Analyze(Scope scope,
                                              NameProvider provider,
                                              PsDectorPattern node)
        {
            if (!scope.TryGetNamedType(node.TypeName, out var typeDefn))
            {
                throw Analyzer.Error("Type " + node.TypeName + " is undefined.", node.Position);
            }

            var dataTypeDefn = typeDefn as AzDataTypeDefn;

            int numElements = dataTypeDefn.Expression.Elements.Count;

            var vars = node.Variables.Select(v => IAzPattern.Analyze(scope, provider, v)).ToList();
            if (vars.Count != numElements)
            {
                throw Analyzer.Error(string.Format("Type {0} has {1} members.", node.TypeName, numElements), node.Position);
            }

            return new(dataTypeDefn, vars, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            // Initialize Type.
            Substitution uniqueSubstitution;
            if (TypeDefn.ParentUnion != null)
            {
                uniqueSubstitution = TypeDefn.ParentUnion.ToCtor().UniqueTvSubstitution(provider);
                Type = TypeDefn.ParentUnion.ToCtor().Substitute(uniqueSubstitution);
            }
            else
            {
                uniqueSubstitution = TypeDefn.ToCtor().UniqueTvSubstitution(provider);
                Type = TypeDefn.ToCtor().Substitute(uniqueSubstitution);
            }

            var cs = new ConstraintSet();
            var typeExpn = TypeDefn.Expression.Substitute(uniqueSubstitution) as AzTypeTupleExpn;
            for (int i = 0; i < Variables.Count; i++)
            {
                var csv = Variables[i].Constrain(provider, scope);
                var c = new Constraint(typeExpn.Elements[i], Variables[i].Type, Position);

                cs = IConstraintSet.Union(cs, c, csv);
            }

            return cs;
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var variableNodes = Variables.SelectMany(v => v.GetSubnodes()).ToList();
            return this.ToArr().Concat(variableNodes).ToList();
        }

        public string Print(int i)
        {
            var vs = Variables.Select(v => v.Print(i)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}", TypeDefn.Name, vs);
        }

        public override string ToString() => Print(0);
    }
}
