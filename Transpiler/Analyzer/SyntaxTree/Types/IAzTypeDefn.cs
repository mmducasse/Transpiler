using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzTypeDefn : IAzDefn
    {
        public static IAzTypeDefn Initialize(Scope scope,
                                             IPsTypeDefn node)
        {
            IAzTypeDefn typeDefn = node switch
            {
                PsDataTypeDefn dataType => AzDataTypeDefn.Initialize(scope, dataType),
                PsUnionTypeDefn unionType => AzUnionTypeDefn.Initialize(scope, unionType),
                PsClassTypeDefn classType => AzClassTypeDefn.Initialize(scope, classType),
                _ => throw new NotImplementedException(),
            };

            scope.AddType(typeDefn);

            return typeDefn;
        }

        public static IAzTypeDefn Analyze(Scope fileScope,
                                          TvProvider tvs,
                                          IAzTypeDefn typeDefn,
                                          IPsTypeDefn node)
        {
            return (typeDefn, node) switch
            {
                (AzDataTypeDefn dataType, PsDataTypeDefn dataNode) =>
                    AzDataTypeDefn.Analyze(fileScope, tvs, dataType, dataNode),

                (AzUnionTypeDefn unionType, PsUnionTypeDefn unionNode) =>
                    AzUnionTypeDefn.Analyze(fileScope, tvs, unionType, unionNode),

                (AzClassTypeDefn classType, PsClassTypeDefn classNode) =>
                    AzClassTypeDefn.Analyze(fileScope, tvs, classType, classNode),

                _ => throw new NotImplementedException(),
            };
        }
    }

    //public static class IAzTypeDefnUtil
    //{
    //    public static AzTypeSymbolExpn ToSym(this IAzTypeDefn typeDefn) =>
    //        new(typeDefn, CodePosition.Null);
    //}
}
