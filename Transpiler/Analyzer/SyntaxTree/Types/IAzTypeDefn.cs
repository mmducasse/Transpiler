using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public static IAzTypeDefn Analyze(Scope scope,
                                          IAzTypeDefn typeDefn,
                                          IPsTypeDefn node)
        {
            return (typeDefn, node) switch
            {
                (AzDataTypeDefn dataType, PsDataTypeDefn dataNode) =>
                    AzDataTypeDefn.Analyze(scope, dataType, dataNode),
                (AzUnionTypeDefn unionType, PsUnionTypeDefn unionNode) =>
                    AzUnionTypeDefn.Analyze(scope, unionType, unionNode),
                (AzClassTypeDefn classType, PsClassTypeDefn classNode) =>
                    AzClassTypeDefn.Analyze(scope, classType, classNode),
                _ => throw new NotImplementedException(),
            };
        }
    }

    public static class IAzTypeDefnUtil
    {
        public static AzTypeSymbolExpn ToSym(this IAzTypeDefn typeDefn) =>
            new(typeDefn, CodePosition.Null);
    }
}
