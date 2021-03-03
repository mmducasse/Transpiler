using static Transpiler.Analysis.EqClass;
using static Transpiler.Analysis.NumClass;
using static Transpiler.Analysis.OrdClass;
using static Transpiler.Analysis.ListDataType;
using System.Collections.Generic;
using static Transpiler.CodePosition;

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

        public Scope Scope => Module.Scope;

        public static Core Instance { get; private set; }

        public Core()
        {
            Instance = this;
            TvProvider tvs = new();

            Module = new Module(" ", "Core");
            Module.Scope = new();

            Int = AzPrimitiveTypeDefn.Make(Module.Scope, "Int");
            Real = AzPrimitiveTypeDefn.Make(Module.Scope, "Real");
            Char = AzPrimitiveTypeDefn.Make(Module.Scope, "Char");

            Bool = AzUnionTypeDefn.Make(Module.Scope, tvs, "Bool", "True", "False");

            Eq = CreateEq(Module.Scope);
            Num = CreateNum(Module.Scope);
            Ord = CreateOrd(Module.Scope);

            List = CreateList(Module.Scope);

            //CreateMiscFunctions(Module.Scope);

            if (Compiler.DebugCore)
            {
                Module.Scope.PrintTypes();
                Module.Scope.PrintFunctions();
                Module.Scope.PrintClassInstances();
                Module.Scope.PrintTypeHeirarchy();

                Analyzer.TEMP_PrintFnTypes(Module.Scope);
            }
        }

        //private void CreateMiscFunctions(Scope scope)
        //{
        //    var @void = new AzTypeTupleExpn(new List<IAzTypeExpn>(), Null);
        //    var getcharType = new AzTypeLambdaExpn(@void, Char.ToCtor(), Null);
        //    var getchar = new Operator("getchar", "Getchar", getcharType, Fixity: eFixity.Prefix);
        //    scope.AddFunction(getchar, getcharType);

        //    var stringType = new AzTypeCtorExpn(List, Char.ToCtor().ToArr(), Null);
        //    var readlineType = new AzTypeLambdaExpn(@void, stringType, Null);
        //    var readline = new Operator("readline", "Readline", readlineType, Fixity: eFixity.Prefix);
        //    scope.AddFunction(readline, readlineType);

        //    var putcharType = new AzTypeLambdaExpn(Char.ToCtor(), @void, Null);
        //    var putchar = new Operator("putchar", "Putchar", putcharType, Fixity: eFixity.Prefix);
        //    scope.AddFunction(putchar, putcharType);
        //}

        public static AzFuncDefn CreateFunction(string name, IAzTypeExpn type)
        {
            return new(name, type, eFixity.Infix, true, Null);
        }
    }
}
