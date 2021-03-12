using System.Collections.Generic;
using static Transpiler.Analysis.OperatorUtil;
using static Transpiler.Extensions;
using static Transpiler.Analysis.Core;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public static class ShowClass
    {
        public static AzClassTypeDefn CreateShow(Scope scope)
        {
            var show = MakeShow(scope);

            InstShow(scope, show, Core.Instance.Int);
            InstShow(scope, show, Core.Instance.Real);
            InstShow(scope, show, Core.Instance.Char);

            return show;
        }

        private static AzClassTypeDefn MakeShow(Scope scope)
        {
            var stringType = new AzTypeCtorExpn(Core.Instance.List, Core.Instance.Char.ToCtor().ToArr(), Null);

            var show = new AzClassTypeDefn("Show", new List<AzClassTypeDefn>(), scope, CodePosition.Null);
            var a = new TypeVariable(0, show.ToArr());
            show.TypeVar = a;

            var type = AzTypeLambdaExpn.Make(a, stringType);

            var fShow = CreateFunction("show", type, fixity: eFixity.Prefix);

            show.Functions = RList(fShow);

            scope.AddType(show);
            scope.AddFunction(fShow, fShow.Type);
            return show;
        }

        private static void InstShow(Scope scope,
                                     AzClassTypeDefn show,
                                     IAzDataTypeDefn implementor)
        {
            var fShow = Function2("show", "primShow", implementor);
            var fns = RList(fShow);

            var instDefn = new AzClassInstDefn(show, implementor, new List<TypeVariable>(), fns, Null);

            scope.AddClassInstance(instDefn);
        }
    }
}
