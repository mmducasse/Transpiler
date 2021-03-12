using static Transpiler.Analysis.EqClass;
using static Transpiler.Analysis.NumClass;
using static Transpiler.Analysis.OrdClass;
using static Transpiler.Analysis.ShowClass;
using static Transpiler.Analysis.ListDataType;
using static Transpiler.Analysis.OptionDataType;
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
        public AzUnionTypeDefn Option { get; }
        public AzUnionTypeDefn List { get; }

        public AzClassTypeDefn Eq { get; }
        public AzClassTypeDefn Ord { get; }
        public AzClassTypeDefn Num { get; }
        public AzClassTypeDefn Show { get; }

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

            Option = CreateOption(Module.Scope);
            List = CreateList(Module.Scope);

            Eq = CreateEq(Module.Scope);
            Ord = CreateOrd(Module.Scope);
            Num = CreateNum(Module.Scope);
            Show = CreateShow(Module.Scope);

            CreateMiscFunctions(Module.Scope);

            if (Compiler.DebugCore)
            {
                Module.Scope.PrintTypes();
                Module.Scope.PrintFunctions();
                Module.Scope.PrintClassInstances();
                Module.Scope.PrintTypeHeirarchy();

                Analyzer.TEMP_PrintFnTypes(Module.Scope);
            }
        }

        private void CreateMiscFunctions(Scope scope)
        {
            var voidType = new AzTypeTupleExpn(new List<IAzTypeExpn>(), Null);
            var stringType = new AzTypeCtorExpn(List, Char.ToCtor().ToArr(), Null);
            var optionIntType = new AzTypeCtorExpn(Option, Int.ToCtor().ToArr(), Null);
            var optionRealType = new AzTypeCtorExpn(Option, Real.ToCtor().ToArr(), Null);
            var varType = new TypeVariable(0, new List<AzClassTypeDefn>());

            var promptType = new AzTypeLambdaExpn(stringType, stringType, Null);
            var prompt = new Operator("prompt", "Prompt", promptType, Fixity: eFixity.Prefix);
            scope.AddFunction(prompt, promptType);

            var putcharType = new AzTypeLambdaExpn(Char.ToCtor(), voidType, Null);
            var putchar = new Operator("putchar", "Putchar", putcharType, Fixity: eFixity.Prefix);
            scope.AddFunction(putchar, putcharType);

            var parseIntType = new AzTypeLambdaExpn(stringType, optionIntType, Null);
            var parseInt = new Operator("parseInt", "ParseInt", parseIntType, Fixity: eFixity.Prefix);
            scope.AddFunction(parseInt, parseIntType);

            var parseRealType = new AzTypeLambdaExpn(stringType, optionRealType, Null);
            var parseReal = new Operator("parseReal", "ParseReal", parseRealType, Fixity: eFixity.Prefix);
            scope.AddFunction(parseReal, parseRealType);

            var modIntType = AzTypeLambdaExpn.Make(Int.ToCtor(), Int.ToCtor(), Int.ToCtor());
            var modInt = new Operator("mod", "intMod", modIntType, Fixity: eFixity.Infix);
            scope.AddFunction(modInt, modIntType);

            var rawEqType = AzTypeLambdaExpn.Make(varType, varType, Bool.ToCtor());
            var rawEq = new Operator("rawEq", "RawEq", rawEqType, Fixity: eFixity.Prefix);
            scope.AddFunction(rawEq, rawEqType);
        }

        public static AzFuncDefn CreateFunction(string name, IAzTypeExpn type, eFixity fixity = eFixity.Infix)
        {
            return new(name, type, fixity, true, Null);
        }
    }
}
