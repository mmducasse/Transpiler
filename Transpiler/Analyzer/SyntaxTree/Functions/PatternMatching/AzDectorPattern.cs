using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzDectorPattern(AzDataTypeDefn TypeDefn,
                                  IReadOnlyList<AzParam> Variables,
                                  CodePosition Position) : IAzPattern
    {
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

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzDectorPattern node)
        {
            var cs = new ConstraintSet();
            var typeExpn = node.TypeDefn.Expression;
            for (int i = 0; i < node.Variables.Count; i++)
            {
                var tv = tvTable.AddNode(scope, node.Variables[i]);
                var c = new EqualConstraint(tv, typeExpn.Elements[i], node);

                cs = IConstraintSet.Union(cs, c);
            }

            return cs;
        }

        public string Print(int i)
        {
            var vs = Variables.Select(v => v.Print(i)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}", TypeDefn.Name, vs);
        }

        public override string ToString() => Print(0);
    }
}
