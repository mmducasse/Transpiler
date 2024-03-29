﻿using System;
using System.Collections.Generic;
using Transpiler.Analysis;
using Transpiler.Generate;

namespace Transpiler
{
    public interface IScope
    {
        string Name { get; }

        IReadOnlyList<IAzFuncStmtDefn> AllFunctions();

        IReadOnlyDictionary<string, IAzFuncDefn> FuncDefinitions { get; }

        IReadOnlyList<AzClassInstDefn> ClassInstances { get; }

        IReadOnlyDictionary<string, IAzTypeDefn> TypeDefinitions { get; }

        bool TryGetNamedType(string typeName, out IAzTypeDefn type);

        bool TryGetFuncDefn(string symbol, out IAzFuncDefn defn);

        bool TryGetTypeVar(string tvName, out TypeVariable tv);

        bool IsSubtypeOf(IAzTypeDefn subtype, IAzTypeSetDefn supertype);

        bool HasClassLineage(AzClassTypeDefn subclass,
                            AzClassTypeDefn superclass,
                            out IReadOnlyList<AzClassTypeDefn> lineage);

        HashSet<IAzTypeSetDefn> GetSupertypes(IAzTypeDefn subtype);

        bool TryGetCommonSupertypeOf(IReadOnlyList<IAzTypeExpn> subtypes,
                                     out HashSet<IAzTypeSetDefn> supertypes);

        bool GetGnFuncTypeParamAlreadyExists(Refinement refinement);

        void PrintTypeHeirarchy();
    }

    public class Scope : IScope
    {
        public string Name { get; }

        public List<IScope> Dependencies { get; } = new List<IScope>();

        IReadOnlyDictionary<string, IAzFuncDefn> IScope.FuncDefinitions => FuncDefinitions;
        public Dictionary<string, IAzFuncDefn> FuncDefinitions { get; } = new();

        public Dictionary<IAzFuncDefn, IAzTypeExpn> FuncDefnTypes { get; } = new();

        public IReadOnlyDictionary<string, IAzTypeDefn> TypeDefinitions => mTypeDefinitions;
        private Dictionary<string, IAzTypeDefn> mTypeDefinitions { get; } = new();

        private Dictionary<IAzTypeDefn, HashSet<IAzTypeSetDefn>> SuperTypes { get; } = new();

        public IReadOnlyList<AzClassInstDefn> ClassInstances => mClassInstances;
        private List<AzClassInstDefn> mClassInstances = new();

        private Dictionary<string, TypeVariable> TypeVariables { get; } = new();

        public HashSet<Refinement> GnFuncTypeParams { get; } = new();

        public Scope()
        {
        }

        public Scope(IEnumerable<IScope> dependencies, string name = "<file>")
        {
            Name = name;
            Dependencies.AddRange(dependencies);
        }

        public Scope(IScope parentScope, string name = "<?>")
        {
            Name = name;
            Dependencies.AddRange(parentScope.ToArr());
        }

        public IReadOnlyList<IAzFuncStmtDefn> AllFunctions()
        {
            List<IAzFuncStmtDefn> all = new();
            foreach (var (_, func) in FuncDefinitions)
            {
                if (func is IAzFuncStmtDefn funcStmtDefn)
                {
                    all.Add(funcStmtDefn);
                }
            }

            foreach (var inst in ClassInstances)
            {
                foreach (var func in inst.Functions)
                {
                    if (func is IAzFuncStmtDefn funcStmtDefn)
                    {
                        all.Add(funcStmtDefn);
                    }
                }
            }

            return all;
        }

        public void AddFunction(IAzFuncDefn func, IAzTypeExpn funcType = null)
        {
            if (FuncDefinitions.ContainsKey(func.Name))
            {
                throw Analyzer.Error("Duplicate function definition: " + func.Name, func.Position);
            }

            FuncDefinitions[func.Name] = func;
            if (funcType != null)
            {
                FuncDefnTypes[func] = funcType;
            }
        }

        public void AddType(IAzTypeDefn type)
        {
            mTypeDefinitions[type.Name] = type;
        }

        public void AddClassInstance(AzClassInstDefn instance)
        {
            mClassInstances.Add(instance);
            AddSuperType(instance.Implementor, instance.Class);
        }

        public bool TryGetNamedType(string typeName, out IAzTypeDefn type)
        {
            if (mTypeDefinitions.TryGetValue(typeName, out type))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.TryGetNamedType(typeName, out type))
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryGetFuncDefn(string symbol, out IAzFuncDefn defn)
        {
            if (FuncDefinitions.TryGetValue(symbol, out defn))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.TryGetFuncDefn(symbol, out defn))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddSuperType(IAzTypeDefn subtype, IAzTypeSetDefn supertype)
        {
            if (!IsSubtypeOf(subtype, supertype))
            {
                if (!SuperTypes.ContainsKey(subtype))
                {
                    SuperTypes[subtype] = new();
                }

                var currSupertypes = SuperTypes[subtype];
                
                foreach (var cst in currSupertypes)
                {
                    if (IsSubtypeOf(supertype, cst))
                    {
                        currSupertypes.Remove(cst);
                    }
                }

                currSupertypes.Add(supertype);
            }
        }

        public bool IsSubtypeOf(IAzTypeDefn subtype, IAzTypeSetDefn supertype)
        {
            if (SuperTypes.TryGetValue(subtype, out var supertypes))
            {
                if (supertypes.Contains(supertype))
                {
                    return true;
                }
                else
                {
                    foreach (var st in supertypes)
                    {
                        if (IsSubtypeOf(st, supertype))
                        {
                            return true;
                        }
                    }
                }
            }

            foreach (var d in Dependencies)
            {
                if (d.IsSubtypeOf(subtype, supertype))
                {
                    return true;
                }
            }

            return false;
        }



        public bool HasClassLineage(AzClassTypeDefn subclass,
                                    AzClassTypeDefn superclass,
                                    out IReadOnlyList<AzClassTypeDefn> lineage)
        {
            var lineageList = new List<AzClassTypeDefn> { subclass };
            lineage = lineageList;

            if (SuperTypes.TryGetValue(subclass, out var supertypes))
            {
                foreach (var st in supertypes)
                {
                    if (st == superclass)
                    {
                        lineageList.Add(superclass);
                        return true;
                    }
                    if (st is AzClassTypeDefn classSt &&
                        HasClassLineage(classSt, superclass, out var nextLineage))
                    {
                        lineageList.AddRange(nextLineage);
                        return true;
                    }
                }
            }

            foreach (var d in Dependencies)
            {
                if (d.HasClassLineage(subclass, superclass, out lineage))
                {
                    return true;
                }
            }

            return false;
        }

        public HashSet<IAzTypeSetDefn> GetSupertypes(IAzTypeDefn subtype)
        {
            var supertypes = new HashSet<IAzTypeSetDefn>();
            if (SuperTypes.TryGetValue(subtype, out var localSupertypes))
            {
                supertypes.UnionWith(localSupertypes);
            }

            foreach (var d in Dependencies)
            {
                var externalSupertypes = d.GetSupertypes(subtype);
                supertypes.UnionWith(externalSupertypes);
            }

            return supertypes;
        }

        public bool TryGetCommonSupertypeOf(IReadOnlyList<IAzTypeExpn> subtypes,
                                            out HashSet<IAzTypeSetDefn> supertypes)
        {
            supertypes = null;

            // Gather concrete types from input list.
            List<IAzTypeDefn> concreteTypes = new();
            foreach (var t in subtypes)
            {
                switch (t)
                {
                    case AzTypeCtorExpn ctorExpn:
                        concreteTypes.Add(ctorExpn.TypeDefn);
                        break;
                    case AzLambdaExpn lamExpn:
                    case AzTupleExpn tupleExpn:
                        return false;
                    case TypeVariable:
                        continue;
                    default:
                        throw new NotImplementedException();
                }
            }

            if (concreteTypes.Count == 0) { return false; }

            supertypes = GetSupertypes(concreteTypes[0]);
            foreach (var t in concreteTypes)
            {
                supertypes.IntersectWith(GetSupertypes(t));
            }

            return true;
        }

        private TvProvider mTvProvider = new();
        public IReadOnlyList<TypeVariable> AddTypeVars(IReadOnlyList<string> tvNames)
        {
            List<TypeVariable> tvs = new();
            foreach (string name in tvNames)
            {
                var tv = mTvProvider.Next;
                TypeVariables[name] = tv;
                tvs.Add(tv);
            }

            return tvs;
        }

        public TypeVariable AddTypeVar(string tvName, IReadOnlyList<AzClassTypeDefn> refinements = null)
        {
            var tv = mTvProvider.Next;
            if (refinements != null)
            {
                tv = tv with { Refinements = refinements };
            }

            TypeVariables[tvName] = tv;

            return tv;
        }

        public bool TryGetTypeVar(string tvName, out TypeVariable tv)
        {
            if (TypeVariables.TryGetValue(tvName, out tv))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.TryGetTypeVar(tvName, out tv))
                {
                    return true;
                }
            }

            return false;
        }

        public void PrintFunctions()
        {
            foreach (var func in FuncDefinitions.Values)
            {
                func.PrintSignature();
            }
        }

        public void PrintTypes()
        {
            foreach (var type in TypeDefinitions.Values)
            {
                type.PrintSignature();
            }
        }

        public void PrintClassInstances()
        {
            foreach (var inst in mClassInstances)
            {
                Console.WriteLine(inst.Print(0));
            }
        }

        public void GnAddTypeParam(Refinement refinement)
        {
            GnFuncTypeParams.Add(refinement);
        }

        public bool GetGnFuncTypeParamAlreadyExists(Refinement refinement)
        {
            if (GnFuncTypeParams.Contains(refinement))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.GetGnFuncTypeParamAlreadyExists(refinement))
                {
                    return true;
                }
            }

            return false;
        }

        public void PrintTypeHeirarchy()
        {
            foreach (var subType in SuperTypes.Keys)
            {
                foreach (var supertype in SuperTypes[subType])
                {
                    Console.WriteLine("{0} : {1}", subType.Name, supertype.Name);
                }
            }

            foreach (var d in Dependencies)
            {
                d.PrintTypeHeirarchy();
            }
        }
    }
}
