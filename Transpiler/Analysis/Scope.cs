using System;
using System.Collections.Generic;

namespace Transpiler
{
    public interface IScope
    {
        bool TryGetNamedType(string typeName, out INamedType type);

        bool TryGetFuncDefn(string symbol, out IFuncDefnNode defn);

        bool TryGetFuncDefnType(string symbol, out IType type);

        bool IsSubtypeOf(INamedType subtype, ITypeSet supertype);

        bool VerifySymbols(params string[] symbols);

        void PrintTypeHeirarchy();
    }

    public class Scope : IScope
    {
        public IEnumerable<IScope> Dependencies { get; } = new List<IScope>();

        public Dictionary<string, IFuncDefnNode> FuncDefinitions { get; } = new();

        public Dictionary<string, IType> FuncDefnTypes { get; } = new();

        public IReadOnlyDictionary<string, INamedType> TypeDefinitions => mTypeDefinitions;
        private Dictionary<string, INamedType> mTypeDefinitions { get; } = new();

        private Dictionary<INamedType, HashSet<ITypeSet>> SuperTypes { get; } = new();

        private List<ClassInstance> ClassInstances { get; } = new();

        public Scope()
        {
        }

        public Scope(IEnumerable<IScope> dependencies)
        {
            Dependencies = dependencies;
        }

        public static Scope FunctionScope(IScope parentScope) =>
            new Scope(parentScope.ToArr());

        public void AddType(INamedType type)
        {
            mTypeDefinitions[type.Name] = type;
            if (type is ClassType classType)
            {
                foreach (var superclass in classType.Superclasses)
                {
                    AddSuperType(classType, superclass);
                }

                foreach (var fn in classType.Functions)
                {
                    FuncDefinitions[fn.Name] = fn;
                    FuncDefnTypes[fn.Name] = fn.Type;
                }
            }
            else if (type is UnionType unionType)
            {
                foreach (var subtype in unionType.Subtypes)
                {
                    AddSuperType(subtype, unionType);
                }
            }
        }

        public void AddClassInstance(ClassInstance instance)
        {
            AddSuperType(instance.Implementor, instance.Class);
        }

        public bool TryGetNamedType(string typeName, out INamedType type)
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

        public bool TryGetFuncDefn(string symbol, out IFuncDefnNode defn)
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

        public bool TryGetFuncDefnType(string defnName, out IType type)
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

        public void AddSuperType(INamedType subtype, ITypeSet supertype)
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

        public bool IsSubtypeOf(INamedType subtype, ITypeSet supertype)
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
