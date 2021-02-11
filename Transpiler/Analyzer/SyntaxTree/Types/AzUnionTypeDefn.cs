using System.Collections.Generic;
using static Transpiler.Extensions;
using Transpiler.Parse;
using System.Linq;

namespace Transpiler.Analysis
{
    public class AzUnionTypeDefn : IAzTypeSetDefn
    {
        public string Name { get; }

        public Scope Scope { get; }

        public IReadOnlyList<TypeVariable> Parameters { get; }

        public IReadOnlyList<IAzTypeDefn> Subtypes { get; set; }

        public CodePosition Position { get; }

        public AzUnionTypeDefn(string name,
                               IReadOnlyList<TypeVariable> parameters,
                               Scope scope,
                               CodePosition position)
        {
            Name = name;
            Parameters = parameters;
            Scope = scope;
            Position = position;
        }

        public static AzUnionTypeDefn Make(Scope scope, string name, params IAzTypeDefn[] subtypes)
        {
            var typeDefn = new AzUnionTypeDefn(name, new List<TypeVariable>(), scope, CodePosition.Null);
            scope.AddType(typeDefn);
            return typeDefn;
        }

        public static AzUnionTypeDefn Initialize(Scope parentScope,
                                                 PsUnionTypeDefn node)
        {
            var scope = new Scope(parentScope, "Union Defn");
            var tvs = scope.AddTypeVars(node.TypeParameters);

            var typeDefn = new AzUnionTypeDefn(node.Name, tvs, scope, node.Position);
            parentScope.AddType(typeDefn);

            List<IAzTypeDefn> subtypes = new();
            foreach (var subnode in node.Subtypes)
            {
                subtypes.Add(IAzTypeDefn.Initialize(scope, subnode));
            }

            typeDefn.Subtypes = subtypes;

            return typeDefn;
        }

        public static AzUnionTypeDefn Analyze(Scope scope,
                                              AzUnionTypeDefn unionType,
                                              PsUnionTypeDefn unionNode)
        {
            // Analyze all of the subtype nodes.
            for (int i = 0; i < unionType.Subtypes.Count; i++)
            {
                var type = unionType.Subtypes[i];
                var node = unionNode.Subtypes[i];

                IAzTypeDefn.Analyze(unionType.Scope, type, node);
            }

            // Add the subtypes to the type heirarchy.
            foreach (var subtype in unionType.Subtypes)
            {
                scope.AddSuperType(subtype, unionType);
            }

            return unionType;
        }

        public string Print(int i)
        {
            string ps = Parameters.Select(p => p.Print()).Separate(" ", prepend: " ");
            string s = string.Format("{0}{1} =\n", Name, ps);
            int i1 = i + 1;

            foreach (var t in Subtypes)
            {
                s += string.Format("{0}| {1}\n", Indent(i1), t.Print(i1));
            }

            return s;
        }
    }
}
