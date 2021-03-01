using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;
using static Transpiler.Analysis.Core;

namespace Transpiler.Analysis
{
    public static class OrdClass
    {
        public static AzClassTypeDefn CreateOrd(Scope scope)
        {
            var num = MakeOrd(scope);

            InstOrd(scope, num, Core.Instance.Int, Core.Instance.Bool);
            InstOrd(scope, num, Core.Instance.Real, Core.Instance.Bool);

            return num;
        }

        private static AzClassTypeDefn MakeOrd(Scope scope)
        {
            var @bool = Core.Instance.Bool;
            var eq = Core.Instance.Eq;

            var ord = new AzClassTypeDefn("Ord", eq.ToArr(), scope, CodePosition.Null);
            var a = new TypeVariable(0, ord.ToArr());
            ord.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, @bool.ToCtor());

            var fLt = CreateFunction("<", type); // new AzFuncDefn("<", type, eFixity.Infix, true, CodePosition.Null);
            var fLte = CreateFunction("<=", type); // new AzFuncDefn("<=", type, eFixity.Infix, true, CodePosition.Null);
            var fGt = CreateFunction(">", type); // new AzFuncDefn(">", type, eFixity.Infix, true, CodePosition.Null);
            var fGte = CreateFunction(">=", type); // new AzFuncDefn(">=", type, eFixity.Infix, true, CodePosition.Null);

            ord.Functions = RList(fLt, fLte, fGt, fGte);

            scope.AddType(ord);
            scope.AddSuperType(ord, eq);
            scope.AddFunction(fLt, fLt.Type);
            scope.AddFunction(fLte, fLte.Type);
            scope.AddFunction(fGt, fGt.Type);
            scope.AddFunction(fGte, fGte.Type);
            return ord;
        }

        private static void InstOrd(Scope scope,
                                    AzClassTypeDefn ord,
                                    IAzDataTypeDefn implementor,
                                    IAzDataTypeDefn @bool)
        {
            var flt = Function2("<", "primLt", implementor, implementor, @bool);
            var flte = Function2("<=", "primLte", implementor, implementor, @bool);
            var fgt = Function2(">", "primGt", implementor, implementor, @bool);
            var fgte = Function2(">=", "primGte", implementor, implementor, @bool);
            var fns = RList(flt, flte, fgt, fgte);

            var instDefn = new AzClassInstDefn(ord, implementor, new List<TypeVariable>(), fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
