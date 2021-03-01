using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;
using static Transpiler.Analysis.Core;

namespace Transpiler.Analysis
{
    public static class EqClass
    {
        public static AzClassTypeDefn CreateEq(Scope scope)
        {
            var @bool = Core.Instance.Bool;

            var eq = MakeEq(scope, @bool);

            InstEq(scope, eq, Core.Instance.Int, @bool);
            InstEq(scope, eq, Core.Instance.Real, @bool);
            InstEq(scope, eq, Core.Instance.Char, @bool);

            return eq;
        }

        private static AzClassTypeDefn MakeEq(Scope scope, IAzDataTypeDefn @bool)
        {
            var eq = new AzClassTypeDefn("Eq", new List<AzClassTypeDefn>(), scope, CodePosition.Null);
            var a = new TypeVariable(0, eq.ToArr());
            eq.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, a, @bool.ToCtor());

            var fEq = CreateFunction("==", type); // new AzFuncDefn("==", type, eFixity.Infix, true, CodePosition.Null);
            var fNeq = CreateFunction("!=", type); // new AzFuncDefn("!=", type, eFixity.Infix, true, CodePosition.Null);

            eq.Functions = RList(fEq, fNeq);

            scope.AddType(eq);
            scope.AddFunction(fEq, fEq.Type);
            scope.AddFunction(fNeq, fNeq.Type);
            return eq;
        }

        private static void InstEq(Scope scope,
                                   AzClassTypeDefn eq,
                                   IAzDataTypeDefn implementor,
                                   IAzDataTypeDefn @bool)
        {
            var feq = Function2("==", "primEq", implementor, implementor, @bool);
            var fneq = Function2("!=", "primNeq", implementor, implementor, @bool);
            var fns = RList(feq, fneq);

            var instDefn = new AzClassInstDefn(eq, implementor, new List<TypeVariable>(), fns, CodePosition.Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
