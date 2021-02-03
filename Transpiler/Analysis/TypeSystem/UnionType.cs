using System.Collections.Generic;
using static Transpiler.Extensions;

namespace Transpiler
{
    // Todo: Replace with TypeSet class?
    public class UnionType : ITypeSet, INamedType
    {
        public bool IsSolved => true;

        public static UnionType Make(string name, params INamedType[] subTypes)
        {
            return new UnionType(name, subTypes);
        }

        public string Name { get; }

        public bool Built { get; private set; }

        public IReadOnlyList<INamedType> Subtypes { get; private set; }

        public IReadOnlyList<string> FutureSubtypes { get; private set; }

        public UnionType(string name,
                         IReadOnlyList<string> futureSubtypes)
        {
            Name = name;
            FutureSubtypes = futureSubtypes;
            Built = false;
        }

        public UnionType(string name,
                         IReadOnlyList<INamedType> subtypes)
        {
            Name = name;
            Subtypes = subtypes;
            Built = true;
        }

        public void Build(Scope scope)
        {
            if (Built)
            {
                return;
            }

            List<INamedType> subtypes = new();
            foreach (var name in FutureSubtypes)
            {
                if (!scope.TryGetType(name, out var st))
                {
                    throw Analyzer.Error("Unknown type: " + name, null);
                }
                subtypes.Add(st);
                scope.AddSuperType(st, this);
            }

            Subtypes = subtypes;
            FutureSubtypes = null;
            Built = true;
        }

        public string Print()
        {
            string s = Name + " =\n";
            foreach (var st in Subtypes)
            {
                s += string.Format("{0}| {1}\n", Indent(1), st.Name);
            }

            return s;
        }
    }
}
