using System;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public static class ListDataType
    {
        public static AzUnionTypeDefn CreateList(Scope scope)
        {
            var p = CodePosition.Null;
            var a = TypeVariable.Simple(0);
            var list = new AzUnionTypeDefn("List", a.ToArr(), scope, p);

            var emptyCtor = new AzFuncDefn("Empty", null, eFixity.Prefix, p);
            var empty = new AzDataTypeDefn("Empty", Array.Empty<TypeVariable>(), emptyCtor, list, scope, p);
            empty.Expression = new AzTypeTupleExpn(Array.Empty<IAzTypeExpn>(), p);
            AzDataTypeDefn.CreateConstructor(scope, empty, list);
            scope.AddType(empty);
            scope.AddSuperType(empty, list);

            var nodeCtor = new AzFuncDefn("Node", null, eFixity.Prefix, p);
            var node = new AzDataTypeDefn("Node", Array.Empty<TypeVariable>(), nodeCtor, list, scope, p);
            node.Expression = new AzTypeTupleExpn(RList<IAzTypeExpn>(a, new AzTypeCtorExpn(list, a.ToArr(), p)), p);
            AzDataTypeDefn.CreateConstructor(scope, node, list);
            scope.AddType(node);
            scope.AddSuperType(node, list);

            list.Subtypes = RList(empty, node);
            scope.AddType(list);
            return list;
        }
    }
}
