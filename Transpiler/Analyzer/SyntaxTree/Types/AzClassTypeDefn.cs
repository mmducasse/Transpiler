using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public class AzClassTypeDefn : IAzTypeSetDefn
    {
        public IReadOnlyList<AzClassTypeDefn> Superclasses { get; }

        public TypeVariable TypeVar { get; }

        public Scope Scope { get; }

        //public IReadOnlyList<FuncSignature> Functions { get; set; }

        public string Name { get; }

        public CodePosition Position { get; }

        public AzClassTypeDefn(string name,
                               IReadOnlyList<AzClassTypeDefn> superclasses,
                               TypeVariable typeVar,
                               Scope scope,
                               CodePosition position)
        {
            Name = name;
            Superclasses = superclasses;
            TypeVar = typeVar;
            Scope = scope;
            Position = position;
        }

        public static AzClassTypeDefn Initialize(Scope parentScope,
                                                 PsClassTypeDefn node)
        {
            var scope = new Scope(parentScope, "Class Defn");
            var tv = scope.AddTypeVar(node.TypeVar);

            var classDefn = new AzClassTypeDefn(node.Name, null, tv, scope, node.Position);
            parentScope.AddType(classDefn);

            return classDefn;
        }

        public static AzClassTypeDefn Analyze(Scope scope,
                                              AzClassTypeDefn classType,
                                              PsClassTypeDefn node)
        {


            throw new NotImplementedException();
        }

        public string Print(int i)
        {
            string s = string.Format("type {0} {\n");

            return s;
        }
    }
}
