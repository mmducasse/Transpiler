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
            var type = AzTypeLambdaExpn.Make(implementor.ToCtor(), implementor.ToCtor(), implementor.ToCtor());

            Dictionary<AzFuncDefn, IAzFuncDefn> fns = new();
            AddInstFunc2(fns, num, "+", "add" + implementor.Name, type);
            AddInstFunc2(fns, num, "-", "sub" + implementor.Name, type);
            AddInstFunc2(fns, num, "*", "mul" + implementor.Name, type);
            AddInstFunc2(fns, num, "/", "div" + implementor.Name, type);

            var instDefn = new AzClassInstDefn(num, implementor, new List<TypeVariable>(), fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
