using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public static class OrdClass
    {
        public static AzClassTypeDefn CreateOrd(Scope scope)
        {
            var num = MakeOrd(scope);

            InstOrd(scope, num, CoreTypes.Instance.Int);
            InstOrd(scope, num, CoreTypes.Instance.Real);

            return num;
        }

        private static AzClassTypeDefn MakeOrd(Scope scope)
        {
            var @bool = CoreTypes.Instance.Bool;
            var eq = CoreTypes.Instance.Eq;

            var ord = new AzClassTypeDefn("Ord", scope, CodePosition.Null);
            var a = new TypeVariable(0, RList(ord, eq));
            ord.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, @bool.ToCtor());

            var fLt = new AzFuncDefn("<", type, eFixity.Infix, CodePosition.Null);
            var fLte = new AzFuncDefn("<=", type, eFixity.Infix, CodePosition.Null);
            var fGt = new AzFuncDefn(">", type, eFixity.Infix, CodePosition.Null);
            var fGte = new AzFuncDefn(">=", type, eFixity.Infix, CodePosition.Null);

            ord.Functions = RList(fLt, fLte, fGt, fGte);

            scope.AddType(ord);
            scope.AddFunction(fLt, fLt.ExplicitType);
            scope.AddFunction(fLte, fLte.ExplicitType);
            scope.AddFunction(fGt, fGt.ExplicitType);
            scope.AddFunction(fGte, fGte.ExplicitType);
            return ord;
        }

        private static void InstOrd(Scope scope,
                                    AzClassTypeDefn num,
                                    IAzDataTypeDefn implementor)
        {
            string name = implementor.Name;

            var fLt = Function2("lt" + name, implementor);
            var fLte = Function2("lte" + name, implementor);
            var fGt = Function2("gt" + name, implementor);
            var fGte = Function2("gte" + name, implementor);
            var fns = RList(fLt, fLte, fGt, fGte);

            var instDefn = new AzClassInstDefn(num, implementor, fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
