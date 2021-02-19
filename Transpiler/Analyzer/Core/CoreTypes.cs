using static Transpiler.Analysis.EqClass;
using static Transpiler.Analysis.NumClass;
using static Transpiler.Analysis.OrdClass;
using static Transpiler.Analysis.ListDataType;

namespace Transpiler.Analysis
{
    public class CoreTypes
    {
        public AzPrimitiveTypeDefn Int { get; }
        public AzPrimitiveTypeDefn Real { get; }
        public AzPrimitiveTypeDefn Char { get; }

        public AzUnionTypeDefn Bool { get; }
        public AzUnionTypeDefn List { get; }

        public AzClassTypeDefn Eq { get; }
        public AzClassTypeDefn Num { get; }
        public AzClassTypeDefn Ord { get; }

        //public IReadOnlyList<Operator> Operators => mOperators;
        //private List<Operator> mOperators { get; } = new();

        //public Dictionary<INamedType, HashSet<ITypeSet>> SuperTypes { get; } = new();

        public IScope Scope => mScope;
        private Scope mScope = new Scope();

        public static CoreTypes Instance { get; private set; }

        public CoreTypes()
        {
            Instance = this;

            Int = AzPrimitiveTypeDefn.Make(mScope, "Int");
            Real = AzPrimitiveTypeDefn.Make(mScope, "Real");
            Char = AzPrimitiveTypeDefn.Make(mScope, "Char");

            Bool = AzUnionTypeDefn.Make(mScope, "Bool", "True", "False");

            Eq = CreateEq(mScope);
            Num = CreateNum(mScope);
            Ord = CreateOrd(mScope);

            List = CreateList(mScope);

            MakeMiscFns(mScope);

            mScope.PrintTypes();
            mScope.PrintFunctions();
            mScope.PrintClassInstances();
            mScope.PrintTypeHeirarchy();

            Analyzer.TEMP_PrintFnTypes(mScope);
        }

        private static void MakeMiscFns(Scope scope)
        {
            var a = TypeVariable.Simple(0);

            var fixType = AzTypeLambdaExpn.Make(AzTypeLambdaExpn.Make(a, a), a);
            var fix = new Operator("fix", "Fix", fixType, eFixity.Prefix);
            scope.AddFunction(fix, fix.Type);
        }

        //private void Add(Operator op)
        //{
        //    mOperators.Add(op);
        //    mScope.FuncDefinitions[op.Name] = op;
        //    mScope.FuncDefnTypes[op.Name] = op.Type;
        //}
    }
}
