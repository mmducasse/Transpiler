using System;
using System.Collections.Generic;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public static class OptionDataType
    {
        public static AzUnionTypeDefn CreateOption(Scope scope)
        {
            var p = CodePosition.Null;
            var a = TypeVariable.Simple(0);
            var option = new AzUnionTypeDefn("Option", a.ToArr(), scope, p);

            var noneCtor = new AzFuncDefn("None", null, eFixity.Prefix, true, p);
            var none = new AzDataTypeDefn("None", Array.Empty<TypeVariable>(), noneCtor, new List<AzFuncDefn>(), option, scope, p);
            none.Expression = new AzTypeTupleExpn(Array.Empty<IAzTypeExpn>(), p);
            AzDataTypeDefn.CreateConstructor(scope, none, option);
            scope.AddType(none);
            scope.AddSuperType(none, option);

            var someCtor = new AzFuncDefn("Some", null, eFixity.Prefix, true, p);
            var some = new AzDataTypeDefn("Some", Array.Empty<TypeVariable>(), someCtor, new List<AzFuncDefn>(), option, scope, p);
            some.Expression = new AzTypeTupleExpn(RList<IAzTypeExpn>(a), p);
            AzDataTypeDefn.CreateConstructor(scope, some, option);
            scope.AddType(some);
            scope.AddSuperType(some, option);

            option.Subtypes = RList(none, some);
            scope.AddType(option);
            return option;
        }
    }
}
