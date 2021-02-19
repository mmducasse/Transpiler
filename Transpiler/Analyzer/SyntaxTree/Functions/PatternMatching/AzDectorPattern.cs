using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzDectorPattern(AzDataTypeDefn TypeDefn,
                                  IReadOnlyList<AzParam> Variables,
                                  CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; set; }

        public static AzDectorPattern Analyze(Scope scope,
                                              PsDectorPattern node)
        {
            if (!scope.TryGetNamedType(node.TypeName, out var typeDefn))
            {
                throw Analyzer.Error("Type " + node.TypeName + " is undefined.", node.Position);
            }

            var dataTypeDefn = typeDefn as AzDataTypeDefn;

            int elements = dataTypeDefn.Expression.Elements.Count;

            var vars = node.Variables.Select(v => AzParam.Analyze(scope, v)).ToList();
            if (vars.Count != elements)
            {
                throw Analyzer.Error(string.Format("Type {0} has {1} members.", node.TypeName, elements), node.Position);
            }

            return new(dataTypeDefn, vars, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            // Initialize Type.
            if (TypeDefn.ParentUnion != null)
            {
                Type = TypeDefn.ParentUnion.ToCtor();
            }
            else
            {
                Type = TypeDefn.ToCtor();
            }

            var cs = new ConstraintSet();
            var typeExpn = TypeDefn.Expression;
            for (int i = 0; i < Variables.Count; i++)
            {
                var c = new Constraint(provider.Next, typeExpn.Elements[i], this);

                cs = IConstraintSet.Union(cs, c);
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
