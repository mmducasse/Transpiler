using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Analysis;
using static Transpiler.Extensions;

namespace Transpiler.Generate
{
    public record GnAppExpn(IGnFuncExpn Function,
                            IGnFuncExpn Argument) : IGnFuncExpn
    {
        public static GnAppExpn Prepare(IScope scope, AzAppExpn appExpn)
        {
            var func = IGnFuncExpn.Prepare(scope, appExpn.Function);
            var arg = IGnFuncExpn.Prepare(scope, appExpn.Argument);

            //var tvs = appExpn.Function.Type.GetTypeVars();
            if (appExpn.Function is AzSymbolExpn symExpn && 
                symExpn.Definition.ExplicitType != null &&
                symExpn.Definition.ExplicitType.GetTypeVars().Where(tv => tv.HasRefinements).Count() > 0)
            {
                var typeMapping = MapDefnTvsToCallTypes(symExpn.Definition, symExpn);
                foreach (var (defnType, expnType) in typeMapping)
                {
                    string typeParamName = CreateTypeArgString(scope, defnType, expnType);
                    var param = new GnSymbolExpn(typeParamName);
                    func = new GnAppExpn(func, param);
                }

                //var funcDefnType = symExpn.Definition.ExplicitType.GetTypeVars().First();
                //foreach (var tv in tvs)
                //{
                //    foreach (var r in tv.Refinements)
                //    {
                //        string typeParamName = CreateTypeArgString(r, null);
                //        var param = new GnSymbolExpn("d" + r.Name + tv.Name);
                //        func = new GnAppExpn(func, param);
                //    }
                //}
            }
            return new(func, arg);
        }

        public static IReadOnlyDictionary<TypeVariable, IAzTypeExpn>
            MapDefnTvsToCallTypes(IAzFuncDefn funcDefn, IAzFuncExpn funcExpn)
        {
            var funcDefnType = funcDefn.ExplicitType;
            var funcExpnType = funcExpn.Type;

            Dictionary<TypeVariable, IAzTypeExpn> mapping = new();

            MapNext(funcDefnType, funcExpnType);

            return mapping;

            void MapNext(IAzTypeExpn defnTypeExpn, IAzTypeExpn expnTypeExpn)
            {
                switch (defnTypeExpn, expnTypeExpn)
                {
                    case (TypeVariable dTv, _):
                        mapping[dTv] = expnTypeExpn;
                        break;

                    case (AzTypeLambdaExpn dLam, AzTypeLambdaExpn eLam):
                        MapNext(dLam.Input, eLam.Input);
                        MapNext(dLam.Output, eLam.Output);
                        break;

                    // Todo: add ctor and tuple cases.
                    case (AzTypeCtorExpn dCtor, AzTypeCtorExpn eCtor):
                        if (dCtor.TypeDefn != eCtor.TypeDefn) { throw new Exception(); }
                        break;

                    default:
                        throw new System.Exception();
                }
            }
        }

        private static string CreateTypeArgString(IScope scope, TypeVariable defnType, IAzTypeExpn expnType)
        {
            var dRef = defnType.Refinements[0];

            if (expnType is AzTypeCtorExpn ctorExpn)
            {
                return string.Format("{0}_{1}", dRef.Name, ctorExpn.TypeDefn.Name);
            }
            else if (expnType is TypeVariable eTv)
            {
                var eRef = eTv.Refinements[0];
                string dictName = string.Format("d{0}{1}", eRef.Name, eTv.Name);
                if (dRef == eRef)
                {
                    return dictName;
                }

                if (scope.HasClassLineage(eRef, dRef, out var lineage))
                {
                    string s = dictName;
                    for (int i = 1; i < lineage.Count; i++)
                    {
                        string funcName = string.Format("{0}From{1}", lineage[i].Name, lineage[i - 1].Name);
                        s = string.Format("{0}({1})", funcName, s.Generated());
                    }
                    return s;
                }
            }

            throw new Exception();
        }

        public string Generate(int i, NameProvider names, ref string s)
        {
            string func;
            if (Function is IGnInlineNode funcSym)
            {
                func = funcSym.Generate();
            }
            else
            {
                func = Function.Generate(i, names, ref s);
            }

            string arg;
            if (Argument is IGnInlineNode argSym)
            {
                arg = argSym.Generate();
            }
            else
            {
                arg = Argument.Generate(i, names, ref s);
            }

            string appRes = names.Next;
            s += string.Format("{0}var {1} = {2}({3})\n", Indent(i), appRes, func, arg);
            return appRes;
        }
    }
}
