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
            string name = implementor.Name;

            var fIntAdd = Function2("add" + name, implementor);
            var fIntSub = Function2("sub" + name, implementor);
            var fIntMul = Function2("mul" + name, implementor);
            var fIntDiv = Function2("div" + name, implementor);
            var fns = RList(fIntAdd, fIntSub, fIntMul, fIntDiv);

            var instDefn = new AzClassInstDefn(num, implementor, fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
