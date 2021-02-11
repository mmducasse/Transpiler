using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public class AzDataTypeDefn : IAzTypeDefn
    {
        public string Name { get; }

        public IReadOnlyList<TypeVariable> Parameters { get; }

        public IAzTypeExpn Expression { get; set; }

        public Scope Scope { get; }

        public CodePosition Position { get; }

        public AzDataTypeDefn(string name,
                              IReadOnlyList<TypeVariable> parameters,
                              Scope scope,
                              CodePosition position)
        {
            Name = name;
            Parameters = parameters;
            Scope = scope;
            Position = position;
        }

        public static AzDataTypeDefn Make(Scope scope, string name)
        {
            var typeDefn = new AzDataTypeDefn(name, new List<TypeVariable>(), scope, CodePosition.Null);
            scope.AddType(typeDefn);
            return typeDefn;
        }

        public static AzDataTypeDefn Initialize(Scope parentScope,
                                                PsDataTypeDefn node)
        {
            var scope = new Scope(parentScope, "Data Defn");
            var tvs = scope.AddTypeVars(node.TypeParameters);

            var typeDefn = new AzDataTypeDefn(node.Name, tvs, scope, node.Position);
            parentScope.AddType(typeDefn);

            return typeDefn;
        }

        public static AzDataTypeDefn Analyze(Scope scope,
                                             AzDataTypeDefn dataType,
                                             PsDataTypeDefn node)
        {
            dataType.Expression = IAzTypeExpn.Analyze(dataType.Scope, node.Expression);
            return dataType;
        }

        public string Print(int i)
        {
            string parameters = Parameters.Select(p => p.Print()).Separate(" ", prepend: " ");
            return string.Format("{0}{1} = {2}", Name, parameters, Expression?.Print(i));
        }
    }
}
