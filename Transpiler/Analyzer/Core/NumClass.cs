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

            InstNum(scope, num, Core.Instance.Int);
            InstNum(scope, num, Core.Instance.Real);

            return num;
        }

        private static AzClassTypeDefn MakeNum(Scope scope)
        {
            var eq = Core.Instance.Eq;
            var num = new AzClassTypeDefn("Num", eq.ToArr(), scope, CodePosition.Null);
            var a = new TypeVariable(0, num.ToArr());
            num.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, a);

            var fAdd = CreateFunction("+", type); // new AzFuncDefn("+", type, eFixity.Infix, true, CodePosition.Null);
            var fSub = CreateFunction("-", type); // new AzFuncDefn("-", type, eFixity.Infix, true, CodePosition.Null);
            var fMul = CreateFunction("*", type); // new AzFuncDefn("*", type, eFixity.Infix, true, CodePosition.Null);
            var fDiv = CreateFunction("/", type); // new AzFuncDefn("/", type, eFixity.Infix, true, CodePosition.Null);

            num.Functions = RList(fAdd, fSub, fMul, fDiv);

            scope.AddType(num);
            scope.AddSuperType(num, eq);
            scope.AddFunction(fAdd, fAdd.Type);
            scope.AddFunction(fSub, fSub.Type);
            scope.AddFunction(fMul, fMul.Type);
            scope.AddFunction(fDiv, fDiv.Type);
            return num;
        }

        private static void InstNum(Scope scope,
                                    AzClassTypeDefn num,
                                    IAzDataTypeDefn implementor)
        {
            var fadd = Function2("+", "primAdd", implementor);
            var fsub = Function2("-", "primSub", implementor);
            var fmul = Function2("*", "primMul", implementor);
            var fdiv = Function2("/", "primDiv", implementor);
            var fns = RList(fadd, fsub, fmul, fdiv);

            var instDefn = new AzClassInstDefn(num, implementor, new List<TypeVariable>(), fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
