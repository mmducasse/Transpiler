using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;
using static Transpiler.Analysis.Core;

namespace Transpiler.Analysis
{
    public static class NumClass
    {
        public static AzClassTypeDefn CreateNum(Scope scope)
        {
            var num = MakeNum(scope);

            InstNum(scope, num, Core.Instance.Int, "int");
            InstNum(scope, num, Core.Instance.Real, "real");

            return num;
        }

        private static AzClassTypeDefn MakeNum(Scope scope)
        {
            var ord = Core.Instance.Ord;
            var num = new AzClassTypeDefn("Num", ord.ToArr(), scope, CodePosition.Null);
            var a = new TypeVariable(0, num.ToArr());
            num.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, a);

            var fAdd = CreateFunction("+", type);
            var fSub = CreateFunction("-", type);
            var fMul = CreateFunction("*", type);
            var fDiv = CreateFunction("/", type);

            num.Functions = RList(fAdd, fSub, fMul, fDiv);

            scope.AddType(num);
            scope.AddSuperType(num, ord);
            scope.AddFunction(fAdd, fAdd.Type);
            scope.AddFunction(fSub, fSub.Type);
            scope.AddFunction(fMul, fMul.Type);
            scope.AddFunction(fDiv, fDiv.Type);
            return num;
        }

        private static void InstNum(Scope scope,
                                    AzClassTypeDefn num,
                                    IAzDataTypeDefn implementor,
                                    string prefix)
        {
            var fadd = Function2("+", prefix + "Add", implementor);
            var fsub = Function2("-", prefix + "Sub", implementor);
            var fmul = Function2("*", prefix + "Mul", implementor);
            var fdiv = Function2("/", prefix + "Div", implementor);
            var fns = RList(fadd, fsub, fmul, fdiv);

            var instDefn = new AzClassInstDefn(num, implementor, new List<TypeVariable>(), fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
