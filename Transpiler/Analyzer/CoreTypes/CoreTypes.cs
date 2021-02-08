using System;
using System.Collections.Generic;
using Transpiler.Analysis;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;

namespace Transpiler
{
    public class CoreTypes
    {
        public PrimitiveType Int { get; } = new("Int");
        public PrimitiveType Real { get; } = new("Real");
        //public PrimitiveType Num { get; } = new("Num");

        public DataType True { get; } = DataType.Make("True");
        public DataType False { get; } = DataType.Make("False");
        public UnionType Bool { get; }

        public IClassType Eq { get; }
        public IClassType Num { get; }

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

            Eq = MakeEq();
            Num = MakeNum();

            Types = new INamedType[]
            {
                Int, Real, Bool, True, False, Eq, Num
            };

            foreach (var type in Types)
            {
                mScope.AddType(type);
            }

            mScope.AddClassInstance(ImplEqInt());
            mScope.AddClassInstance(ImplNumInt());
            //mScope.AddClassInstance(ImplEqReal());
            //mScope.AddClassInstance(ImplNumReal());

            foreach (var t in Types)
            {
                Console.WriteLine(t.Print(false));
            }
            mScope.PrintTypeHeirarchy();
        }

        private IClassType MakeEq()
        {
            var cEq = new ClassType("Eq");

            var a = new TypeVariable(1, cEq.ToArr());

            var type = FunType.Make(a, a, Bool);

            var fEq = new FuncSignature("==", type);
            var fNeq = new FuncSignature("!=", type);

            cEq.TypeVar = a;
            cEq.Functions = RList(fEq, fNeq);

            return cEq;
        }

        private IClassType MakeNum()
        {
            var cNum = new ClassType("Num");

            var a = new TypeVariable(1, cNum.ToArr());

            var type = FunType.Make(a, a, a);

            var fAdd = new FuncSignature("+", type);
            var fSub = new FuncSignature("-", type);
            var fMul = new FuncSignature("*", type);
            var fDiv = new FuncSignature("/", type);

            cNum.TypeVar = a;
            cNum.Functions = RList(fAdd, fSub, fMul, fDiv);

            return cNum;
        }

        private ClassInstance ImplEqInt()
        {
            var fIntEq = Function2("==", Int, Int, Bool);
            var fIntNeq = Function2("!=", Int, Int, Bool);
            var fns = RList(fIntEq, fIntNeq);

            return new ClassInstance(Eq, Int, fns);
        }

        private ClassInstance ImplNumInt()
        {
            var fIntAdd = Function2("+", Int, Int, Int);
            var fIntSub = Function2("-", Int, Int, Int);
            var fIntMul = Function2("*", Int, Int, Int);
            var fIntDiv = Function2("/", Int, Int, Int);
            var fns = RList(fIntAdd, fIntSub, fIntMul, fIntDiv);

            return new ClassInstance(Num, Int, fns);
        }

        private ClassInstance ImplEqReal()
        {
            var fRealEq = Function2("==", Real, Real, Bool);
            var fRealNeq = Function2("!=", Real, Real, Bool);
            var fns = RList(fRealEq, fRealNeq);

            return new ClassInstance(Eq, Real, fns);
        }

        private ClassInstance ImplNumReal()
        {
            var fRealAdd = Function2("+", Real, Real, Real);
            var fRealSub = Function2("-", Real, Real, Real);
            var fRealMul = Function2("*", Real, Real, Real);
            var fRealDiv = Function2("/", Real, Real, Real);
            var fns = RList(fRealAdd, fRealSub, fRealMul, fRealDiv);

            return new ClassInstance(Num, Real, fns);
        }

        private void Add(Operator op)
        {
            mOperators.Add(op);
            mScope.FuncDefinitions[op.Name] = op;
            mScope.FuncDefnTypes[op.Name] = op.Type;
        }
    }
}
