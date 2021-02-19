using System;
using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public interface IScope
    {
        string Name { get; }

        IReadOnlyDictionary<string, IAzFuncDefn> FuncDefinitions { get; }

        IReadOnlyList<AzClassInstDefn> ClassInstances { get; }

        bool TryGetNamedType(string typeName, out IAzTypeDefn type);

        bool TryGetFuncDefn(string symbol, out IAzFuncDefn defn);

        bool TryGetTypeVar(string tvName, out TypeVariable tv);

        bool IsSubtypeOf(IAzTypeDefn subtype, IAzTypeSetDefn supertype);

        HashSet<IAzTypeSetDefn> GetSupertypes(IAzTypeDefn subtype);

        bool TryGetCommonSupertypeOf(IReadOnlyList<IAzTypeExpn> subtypes,
                                     out HashSet<IAzTypeSetDefn> supertypes);

        void PrintTypeHeirarchy();
    }

    public class Scope : IScope
    {
        public string Name { get; }

        public IEnumerable<IScope> Dependencies { get; } = new List<IScope>();

        IReadOnlyDictionary<string, IAzFuncDefn> IScope.FuncDefinitions => FuncDefinitions;
        public Dictionary<string, IAzFuncDefn> FuncDefinitions { get; } = new();

        public Dictionary<IAzFuncDefn, IAzTypeExpn> FuncDefnTypes { get; } = new();

        public IReadOnlyDictionary<string, IAzTypeDefn> TypeDefinitions => mTypeDefinitions;
        private Dictionary<string, IAzTypeDefn> mTypeDefinitions { get; } = new();

        private Dictionary<IAzTypeDefn, HashSet<IAzTypeSetDefn>> SuperTypes { get; } = new();

        public IReadOnlyList<AzClassInstDefn> ClassInstances => mClassInstances;
        private List<AzClassInstDefn> mClassInstances = new();

        private Dictionary<string, TypeVariable> TypeVariables { get; } = new();

        public Scope()
        {
        }

        public Scope(IEnumerable<IScope> dependencies, string name = "<file>")
        {
            Name = name;
            Dependencies = dependencies;
        }

        public Scope(IScope parentScope, string name = "<?>")
        {
            Name = name;
            Dependencies = parentScope.ToArr();
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
                Console.WriteLine(func.Print(0));
            }
        }

        public void PrintTypes()
        {
            foreach (var type in TypeDefinitions.Values)
            {
                Console.WriteLine(type.Print(0));
            }
        }

        public void PrintClassInstances()
        {
            foreach (var inst in mClassInstances)
            {
                Console.WriteLine(inst.Print(0));
            }
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
