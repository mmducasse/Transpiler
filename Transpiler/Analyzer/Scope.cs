using System;
using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public interface IScope
    {
        string Name { get; }

        bool TryGetNamedType(string typeName, out IAzTypeDefn type);

        bool TryGetFuncDefn(string symbol, out IAzFuncDefn defn);

        bool TryGetFuncDefnType(string symbol, out IAzTypeExpn type);

        bool TryGetTypeVar(string tvName, out TypeVariable tv);

        bool IsSubtypeOf(IAzTypeDefn subtype, IAzTypeSetDefn supertype);

        bool VerifySymbols(params string[] symbols);

        void PrintTypeHeirarchy();
    }

    public class Scope : IScope
    {
        public string Name { get; }

        public IEnumerable<IScope> Dependencies { get; } = new List<IScope>();

        public Dictionary<string, IAzFuncDefn> FuncDefinitions { get; } = new();

        public Dictionary<string, IAzTypeExpn> FuncDefnTypes { get; } = new();

        public IReadOnlyDictionary<string, IAzTypeDefn> TypeDefinitions => mTypeDefinitions;
        private Dictionary<string, IAzTypeDefn> mTypeDefinitions { get; } = new();

        private Dictionary<IAzTypeDefn, HashSet<IAzTypeSetDefn>> SuperTypes { get; } = new();

        private List<AzClassInstDefn> ClassInstances { get; } = new();

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

        public void AddFunction(IAzFuncDefn func)
        {
            if (FuncDefinitions.ContainsKey(func.Name))
            {
                throw Analyzer.Error("Duplicate function definition: " + func.Name, func.Position);
            }
            FuncDefinitions[func.Name] = func;
        }

        public void AddType(IAzTypeDefn type)
        {
            mTypeDefinitions[type.Name] = type;
            //if (type is ClassType classType)
            //{
            //    foreach (var superclass in classType.Superclasses)
            //    {
            //        AddSuperType(classType, superclass);
            //    }

                //    foreach (var fn in classType.Functions)
                //    {
                //        FuncDefinitions[fn.Name] = fn;
                //        FuncDefnTypes[fn.Name] = fn.Type;
                //    }
                //}
        }

        public void AddClassInstance(AzClassInstDefn instance)
        {
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

        public bool TryGetFuncDefnType(string defnName, out IAzTypeExpn type)
        {
            if (FuncDefnTypes.TryGetValue(defnName, out type))
            {
                return true;
            }

            foreach (var d in Dependencies)
            {
                if (d.TryGetFuncDefnType(defnName, out type))
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

        public TypeVariable AddTypeVar(string tvName)
        {
            var tv = mTvProvider.Next;
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

        public bool VerifySymbols(params string[] symbols)
        {
            foreach (string s in symbols)
            {
                if (!FuncDefinitions.ContainsKey(s))
                {
                    foreach (var d in Dependencies)
                    {
                        if (!d.VerifySymbols(s))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
