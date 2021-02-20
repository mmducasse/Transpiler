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

            InstNum(scope, num, CoreTypes.Instance.Int);
            InstNum(scope, num, CoreTypes.Instance.Real);

            return num;
        }

        private static AzClassTypeDefn MakeNum(Scope scope)
        {
            var num = new AzClassTypeDefn("Num", scope, CodePosition.Null);
            var eq = CoreTypes.Instance.Eq;
            var a = new TypeVariable(0, RList(num, eq));
            num.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, a);

            var fAdd = new AzFuncDefn("+", type, eFixity.Infix, CodePosition.Null);
            var fSub = new AzFuncDefn("-", type, eFixity.Infix, CodePosition.Null);
            var fMul = new AzFuncDefn("*", type, eFixity.Infix, CodePosition.Null);
            var fDiv = new AzFuncDefn("/", type, eFixity.Infix, CodePosition.Null);

            num.Functions = RList(fAdd, fSub, fMul, fDiv);

            scope.AddType(num);
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
