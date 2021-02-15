using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public class AzClassTypeDefn : IAzTypeSetDefn
    {
        public IReadOnlyList<AzClassTypeDefn> Superclasses =>
            TypeVar.Refinements.Where(s => s != this).ToList();

        public TypeVariable TypeVar { get; set; }

        public Scope Scope { get; }

        public IReadOnlyList<AzFuncDefn> Functions { get; set; }

        public string Name { get; }

        public CodePosition Position { get; }

        public AzClassTypeDefn(string name,
                               Scope scope,
                               CodePosition position)
        {
            Name = name;
            Scope = scope;
            Position = position;
        }

        public static AzClassTypeDefn Initialize(Scope fileScope,
                                                 PsClassTypeDefn node)
        {
            var scope = new Scope(fileScope, "Class Defn");
            var tv = scope.AddTypeVar(node.TypeVar);

            var classDefn = new AzClassTypeDefn(node.Name, scope, node.Position);
            classDefn.TypeVar = tv with { Refinements = classDefn.ToArr() };
            fileScope.AddType(classDefn);

            // Analyze the class's functions.
            List<AzFuncDefn> funcDefns = new();
            foreach (var funcNode in node.Functions)
            {
                var funcDefn = AzFuncDefn.Initialize(scope, funcNode);
                funcDefns.AddRange(funcDefn);
            }
            foreach (var funcDefn in funcDefns)
            {
                fileScope.AddFunction(funcDefn, funcDefn.ExplicitType);
            }

            classDefn.Functions = funcDefns;

            return classDefn;
        }

        public static AzClassTypeDefn Analyze(Scope scope,
                                              AzClassTypeDefn classType,
                                              PsClassTypeDefn node)
        {
            for (int i = 0; i < classType.Functions.Count; i++)
            {
                var funcDefn = classType.Functions[i];
                var funcNode = node.Functions[i];
                AzFuncDefn.Analyze(classType.Scope, funcDefn, funcNode);
            }

            return classType;
        }

        public string Print(int i)
        {
            var supers = Superclasses.Select(s => string.Format("{0} {1}", s.Name, TypeVar.Print())).Separate(", ", append: " => ");
            string s = string.Format("type {0}{1} {2} {{\n", supers, Name, TypeVar.Print());

            foreach (var funcDecl in Functions)
            {
                s += string.Format("{0}{1}\n", Indent(1), funcDecl.Print(0));
            }

            return s;
        }

        public override string ToString() => Name;
    }
}
