using System.Collections.Generic;
using Transpiler.Analysis;
using static Transpiler.Analysis.OperatorUtil;

namespace Transpiler
{
    public class CoreTypes : IScope
    {
        public PrimitiveType Int { get; } = new("Int");
        public PrimitiveType Real { get; } = new("Real");

        public DataType True { get; } = DataType.Make("True");
        public DataType False { get; } = DataType.Make("False");
        public UnionType Bool { get; } = UnionType.Make("Bool", "True", "False");

        public INamedType[] Types { get; }

        public IReadOnlyList<Operator> Operators => mOperators;
        private List<Operator> mOperators { get; } = new();

        public static CoreTypes Instance { get; private set; }

        public CoreTypes()
        {
            Instance = this;

            Types = new INamedType[]
            {
                Int, Real, Bool, True, False
            };

            // Int functions.
            Add(Function2("+", Int));
            Add(Function2("-", Int));
        }

        private void Add(Operator defn)
        {
            mOperators.Add(defn);
        }

        public bool TryGetType(string typeName, out INamedType type)
        {
            type = null;

            foreach (var t in Types)
            {
                if (t.Name == typeName)
                {
                    type = t;
                    return true;
                }
            }

            return false;
        }

        public bool TryGetTypeForDefnName(string symbol, out IType type)
        {
            type = null;

            foreach (var op in mOperators)
            {
                if (op.Name == symbol)
                {
                    type = op.Type;
                    return true;
                }
            }

            return false;
        }

        public bool VerifySymbols(params string[] symbols)
        {
            throw new System.NotImplementedException();
        }
    }
}
