using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public class AzClassTypeDefn : IAzTypeSetDefn
    {
        public IReadOnlyList<AzClassTypeDefn> Superclasses { get; }

        public TypeVariable TypeVar { get; set; }

        public Scope Scope { get; }

        public IReadOnlyList<AzFuncDefn> Functions { get; set; }

        public string Name { get; }

        public CodePosition Position { get; }

        public AzClassTypeDefn(string name,
                               IReadOnlyList<AzClassTypeDefn> superclasses,
                               Scope scope,
                               CodePosition position)
        {
            Name = name;
            Superclasses = superclasses;
            Scope = scope;
            Position = position;
        }

        public static AzClassTypeDefn Initialize(Scope fileScope,
                                                 PsClassTypeDefn node)
        {
            var scope = new Scope(fileScope, "Class Defn");
            var classDefn = new AzClassTypeDefn(node.Name, new List<AzClassTypeDefn>(), scope, node.Position);

            var refinements = new List<AzClassTypeDefn> { classDefn };
            var tv = scope.AddTypeVar(node.TypeVar, refinements);

            classDefn.TypeVar = tv;
            fileScope.AddType(classDefn);

            // Apply refinements to the type varaible.
            AzTypeRefinementGroup.AnalyzeRefinements(scope, node.Refinements);

            // Analyze the class's functions.
            List<AzFuncDefn> funcDefns = new();
            foreach (var funcNode in node.Functions)
            {
                var funcDefn = AzFuncDefn.Initialize(scope, funcNode);
                funcDefns.Add(funcDefn);
            }
            foreach (var funcDefn in funcDefns)
            {
                fileScope.AddFunction(funcDefn, funcDefn.Type);
            }

            classDefn.Functions = funcDefns;

            return classDefn;
        }

        public static AzClassTypeDefn Analyze(Scope scope,
                                              TvProvider tvs,
                                              AzClassTypeDefn classType,
                                              PsClassTypeDefn node)
        {
            for (int i = 0; i < classType.Functions.Count; i++)
            {
                var funcDefn = classType.Functions[i];
                var funcNode = node.Functions[i];

                if (funcNode.Expression != null)
                {
                    // Analyze the function if it has a default implementation.
                    AzFuncDefn.Analyze(classType.Scope, new("p"), funcDefn, funcNode);
                }
            }

            return classType;
        }

        public bool TryGetFunction(string name, out AzFuncDefn funcDefn)
        {
            funcDefn = null;

            foreach (var fn in Functions)
            {
                if (fn.Name == name)
                {
                    funcDefn = fn;
                    return true;
                }
            }

            return false;
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

        public void PrintSignature()
        {
        }
    }
}
