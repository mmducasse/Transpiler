using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public static class EqClass
    {
        public static AzClassTypeDefn CreateEq(Scope scope)
        {
            var @bool = CoreTypes.Instance.Bool;

            var eq = MakeEq(scope, @bool);

            InstEq(scope, eq, CoreTypes.Instance.Int, @bool);
            InstEq(scope, eq, CoreTypes.Instance.Real, @bool);

            return eq;
        }

        private static AzClassTypeDefn MakeEq(Scope scope, IAzDataTypeDefn @bool)
        {
            var cEq = new AzClassTypeDefn("Eq", scope, CodePosition.Null);
            var a = new TypeVariable(0, cEq.ToArr());
            cEq.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, @bool.ToCtor());

            var fEq = new AzFuncDefn("==", type, eFixity.Infix, CodePosition.Null);
            var fNeq = new AzFuncDefn("!=", type, eFixity.Infix, CodePosition.Null);

            cEq.Functions = RList(fEq, fNeq);

            scope.AddType(cEq);
            scope.AddFunction(fEq, fEq.ExplicitType);
            scope.AddFunction(fNeq, fNeq.ExplicitType);
            return cEq;
        }

        private static void InstEq(Scope scope,
                                   AzClassTypeDefn eq,
                                   IAzDataTypeDefn implementor,
                                   IAzDataTypeDefn @bool)
        {
            var type = AzTypeLambdaExpn.Make(implementor.ToCtor(), implementor.ToCtor(), @bool.ToCtor());

            Dictionary<AzFuncDefn, IAzFuncDefn> fns = new();
            AddInstFunc2(fns, eq, "==", "eq" + implementor.Name, type);
            AddInstFunc2(fns, eq, "!=", "neq" + implementor.Name, type);

            var instDefn = new AzClassInstDefn(eq, implementor, new List<TypeVariable>(), fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
