using System.Collections.Generic;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public interface IClassType : ITypeSet
    {
        IReadOnlyList<ClassType> Superclasses { get; }

        //IReadOnlyList<TypeVariable> TypeVars { get; }
        TypeVariable TypeVar { get; }

        IReadOnlyList<FuncSignature> Functions { get; }
    }

    public class ClassType : IClassType
    {
        public string Name { get; }

        public IReadOnlyList<ClassType> Superclasses { get; }

        //public IReadOnlyList<TypeVariable> TypeVars { get; set; } = new List<TypeVariable>();
        public TypeVariable TypeVar { get; set; }

        public IReadOnlyList<FuncSignature> Functions { get; set; } = new List<FuncSignature>();

        public bool IsSolved => false;

        public ClassType(string name,
                         IReadOnlyList<ClassType> superclasses = null)
        {
            Name = name;
            Superclasses = superclasses ?? new List<ClassType>();
        }

        public string Print(bool terse = true)
        {
            string s = string.Format("{0} {1}", Name, TypeVar.Print());
            if (!terse)
            {
                s += "::\n";
                foreach (var fn in Functions)
                {
                    s += string.Format("{0}{1}\n", Indent(1), fn.Print(1));
                }
            }

            return s;
        }

        public override string ToString()
        {
            return "class " + Name;
        }
    }
}
