﻿using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public static class OrdClass
    {
        public static AzClassTypeDefn CreateOrd(Scope scope)
        {
            var num = MakeOrd(scope);

            InstOrd(scope, num, CoreTypes.Instance.Int, CoreTypes.Instance.Bool);
            InstOrd(scope, num, CoreTypes.Instance.Real, CoreTypes.Instance.Bool);

            return num;
        }

        private static AzClassTypeDefn MakeOrd(Scope scope)
        {
            var @bool = CoreTypes.Instance.Bool;
            var eq = CoreTypes.Instance.Eq;

            var ord = new AzClassTypeDefn("Ord", eq.ToArr(), scope, CodePosition.Null);
            var a = new TypeVariable(0, ord.ToArr());
            ord.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, @bool.ToCtor());

            var fLt = new AzFuncDefn("<", type, eFixity.Infix, CodePosition.Null);
            var fLte = new AzFuncDefn("<=", type, eFixity.Infix, CodePosition.Null);
            var fGt = new AzFuncDefn(">", type, eFixity.Infix, CodePosition.Null);
            var fGte = new AzFuncDefn(">=", type, eFixity.Infix, CodePosition.Null);

            ord.Functions = RList(fLt, fLte, fGt, fGte);

            scope.AddType(ord);
            scope.AddSuperType(ord, eq);
            scope.AddFunction(fLt, fLt.ExplicitType);
            scope.AddFunction(fLte, fLte.ExplicitType);
            scope.AddFunction(fGt, fGt.ExplicitType);
            scope.AddFunction(fGte, fGte.ExplicitType);
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
