using System;
using System.Collections.Generic;
using Transpiler.Analysis;
using static Transpiler.Analysis.OperatorUtil;

namespace Transpiler
{
    public class CoreTypes
    {
        //public PrimitiveType Int { get; } = new("Int");
        //public PrimitiveType Real { get; } = new("Real");
        public PrimitiveType Num { get; } = new("Num");

        public DataType True { get; } = DataType.Make("True");
        public DataType False { get; } = DataType.Make("False");
        public UnionType Bool { get; }

        public INamedType[] Types { get; }

        public IReadOnlyList<Operator> Operators => mOperators;
        private List<Operator> mOperators { get; } = new();

        public Dictionary<INamedType, HashSet<ITypeSet>> SuperTypes { get; } = new();

        public IScope Scope => mScope;
        private Scope mScope = new Scope();

        public static CoreTypes Instance { get; private set; }

        public CoreTypes()
        {
            Instance = this;

            Bool = UnionType.Make("Bool", True, False);

            Types = new INamedType[]
            {
                Num, Bool, True, False
            };

            foreach (var type in Types)
            {
                mScope.TypeDefinitions[type.Name] = type;
            }

            mScope.AddSuperType(True, Bool);
            mScope.AddSuperType(False, Bool);

            // Int functions.
            Add(Function2("+", Num));
            Add(Function2("-", Num));
            Add(Function2("==", Num, Num, Bool));
        }

        private void Add(Operator op)
        {
            mOperators.Add(op);
            mScope.FuncDefinitions[op.Name] = op;
            mScope.FuncDefnTypes[op.Name] = op.Type;
        }
    }
}
