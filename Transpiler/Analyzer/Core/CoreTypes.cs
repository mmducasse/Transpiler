using System;
using System.Collections.Generic;
using Transpiler.Parse;
using Transpiler.Analysis;
using static Transpiler.Extensions;
using static Transpiler.Analysis.OperatorUtil;

namespace Transpiler.Analysis
{
    public class CoreTypes
    {
        public AzPrimitiveTypeDefn Int { get; }
        public AzPrimitiveTypeDefn Real { get; }

        public AzUnionTypeDefn Bool { get; }

        public AzClassTypeDefn Eq { get; }
        public AzClassTypeDefn Num { get; }

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

            Bool = AzUnionTypeDefn.Make(mScope, "Bool", "True", "False");

            Eq = MakeEq();
            Num = MakeNum();

            mScope.AddClassInstance(ImplEqInt());
            mScope.AddClassInstance(ImplNumInt());
            mScope.AddClassInstance(ImplEqReal());
            mScope.AddClassInstance(ImplNumReal());

            MakeMiscFns(mScope);

            mScope.PrintTypes();
            mScope.PrintTypeHeirarchy();
        }

        private static void MakeMiscFns(Scope scope)
        {
            var a = TypeVariable.Simple(0);

            var fixType = AzTypeLambdaExpn.Make(AzTypeLambdaExpn.Make(a, a), a);
            var fix = new Operator("fix", fixType, eFixity.Prefix);
            scope.AddFunction(fix, fix.Type);
        }

        private AzClassTypeDefn MakeEq()
        {
            var cEq = new AzClassTypeDefn("Eq", mScope, CodePosition.Null);
            var a = new TypeVariable(0, cEq.ToArr());
            cEq.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, Bool.ToCtor());

            var fEq = new AzFuncDefn("==", type, eFixity.Infix, CodePosition.Null);
            var fNeq = new AzFuncDefn("!=", type, eFixity.Infix, CodePosition.Null);

            cEq.Functions = RList(fEq, fNeq);

            mScope.AddType(cEq);
            mScope.AddFunction(fEq, fEq.ExplicitType);
            mScope.AddFunction(fNeq, fNeq.ExplicitType);
            return cEq;
        }

        private AzClassTypeDefn MakeNum()
        {
            var cNum = new AzClassTypeDefn("Num", mScope, CodePosition.Null);
            var a = new TypeVariable(0, cNum.ToArr());
            cNum.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, a);

            var fAdd = new AzFuncDefn("+", type, eFixity.Infix, CodePosition.Null);
            var fSub = new AzFuncDefn("-", type, eFixity.Infix, CodePosition.Null);
            var fMul = new AzFuncDefn("*", type, eFixity.Infix, CodePosition.Null);
            var fDiv = new AzFuncDefn("/", type, eFixity.Infix, CodePosition.Null);

            cNum.Functions = RList(fAdd, fSub, fMul, fDiv);

            mScope.AddType(cNum);
            mScope.AddFunction(fAdd, fAdd.ExplicitType);
            mScope.AddFunction(fSub, fSub.ExplicitType);
            mScope.AddFunction(fMul, fMul.ExplicitType);
            mScope.AddFunction(fDiv, fDiv.ExplicitType);
            return cNum;
        }

        private AzClassInstDefn ImplEqInt()
        {
            var fIntEq = Function2("eqInt", Int, Int, Bool);
            var fIntNeq = Function2("neqInt", Int, Int, Bool);
            var fns = RList(fIntEq, fIntNeq);

            return new AzClassInstDefn(Eq, Int, fns, CodePosition.Null);
        }

        private AzClassInstDefn ImplNumInt()
        {
            var fIntAdd = Function2("addInt", Int);
            var fIntSub = Function2("subInt", Int);
            var fIntMul = Function2("mulInt", Int);
            var fIntDiv = Function2("divInt", Int);
            var fns = RList(fIntAdd, fIntSub, fIntMul, fIntDiv);

            return new AzClassInstDefn(Num, Int, fns, CodePosition.Null);
        }

        private AzClassInstDefn ImplEqReal()
        {
            var fRealEq = Function2("eqReal", Real, Real, Bool);
            var fRealNeq = Function2("neqReal", Real, Real, Bool);
            var fns = RList(fRealEq, fRealNeq);

            return new AzClassInstDefn(Eq, Real, fns, CodePosition.Null);
        }

        private AzClassInstDefn ImplNumReal()
        {
            var fRealAdd = Function2("addReal", Real);
            var fRealSub = Function2("subReal", Real);
            var fRealMul = Function2("mulReal", Real);
            var fRealDiv = Function2("divReal", Real);
            var fns = RList(fRealAdd, fRealSub, fRealMul, fRealDiv);

            return new AzClassInstDefn(Num, Real, fns, CodePosition.Null);
        }

        //private void Add(Operator op)
        //{
        //    mOperators.Add(op);
        //    mScope.FuncDefinitions[op.Name] = op;
        //    mScope.FuncDefnTypes[op.Name] = op.Type;
        //}
    }
}
