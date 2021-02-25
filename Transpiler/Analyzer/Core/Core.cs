using static Transpiler.Analysis.EqClass;
using static Transpiler.Analysis.NumClass;
using static Transpiler.Analysis.OrdClass;
using static Transpiler.Analysis.ListDataType;

namespace Transpiler.Analysis
{
    public class Core
    {
        public AzPrimitiveTypeDefn Int { get; }
        public AzPrimitiveTypeDefn Real { get; }
        public AzPrimitiveTypeDefn Char { get; }

        public AzUnionTypeDefn Bool { get; }
        public AzUnionTypeDefn List { get; }

        public AzClassTypeDefn Eq { get; }
        public AzClassTypeDefn Num { get; }
        public AzClassTypeDefn Ord { get; }

        public Module Module { get; }

        public IScope Scope => Module.Scope;

        public static Core Instance { get; private set; }

        public Core()
        {
            Instance = this;

            Module = new Module(" ", "Core");
            Module.Scope = new();

            Int = AzPrimitiveTypeDefn.Make(Module.Scope, "Int");
            Real = AzPrimitiveTypeDefn.Make(Module.Scope, "Real");
            Char = AzPrimitiveTypeDefn.Make(Module.Scope, "Char");

            Bool = AzUnionTypeDefn.Make(Module.Scope, "Bool", "True", "False");

            Eq = CreateEq(Module.Scope);
            Num = CreateNum(Module.Scope);
            Ord = CreateOrd(Module.Scope);

            List = CreateList(Module.Scope);

            //mScope.PrintTypes();
            //mScope.PrintFunctions();
            //mScope.PrintClassInstances();
            //mScope.PrintTypeHeirarchy();

            //Analyzer.TEMP_PrintFnTypes(mScope);
        }
    }
}
