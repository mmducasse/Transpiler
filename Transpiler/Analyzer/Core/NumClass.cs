using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;

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

            var fAdd = new AzFuncDefn("+", type, eFixity.Infix, true, CodePosition.Null);
            var fSub = new AzFuncDefn("-", type, eFixity.Infix, true, CodePosition.Null);
            var fMul = new AzFuncDefn("*", type, eFixity.Infix, true, CodePosition.Null);
            var fDiv = new AzFuncDefn("/", type, eFixity.Infix, true, CodePosition.Null);

            num.Functions = RList(fAdd, fSub, fMul, fDiv);

            scope.AddType(num);
            scope.AddSuperType(num, eq);
            scope.AddFunction(fAdd, fAdd.ExplicitType);
            scope.AddFunction(fSub, fSub.ExplicitType);
            scope.AddFunction(fMul, fMul.ExplicitType);
            scope.AddFunction(fDiv, fDiv.ExplicitType);
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
