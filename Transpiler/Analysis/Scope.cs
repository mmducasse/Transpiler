using System;
using System.Collections.Generic;

namespace Transpiler
{
    public interface IScope
    {
        bool TryGetType(string typeName, out INamedType type);

        bool TryGetTypeForDefnName(string symbol, out IType type);

        bool IsSubtypeOf(INamedType subtype, ITypeSet supertype);

        bool VerifySymbols(params string[] symbols);

        void PrintTypeHeirarchy();
    }

    public class Scope : IScope
    {
        public IScope ParentScope { get; }

        public Dictionary<string, INamedType> TypeDefinitions { get; } = new();

        public Dictionary<INamedType, HashSet<ITypeSet>> SuperTypes { get; } = new();

        public Dictionary<string, IFuncDefnNode> FuncDefinitions { get; } = new();

        public Dictionary<string, IType> FuncDefnTypes { get; } = new();

        public TvTable TvTable { get; }

        public Scope(IScope parentScope = null,
                     TvTable tvTable = null)
        {
            ParentScope = parentScope;
            if ((tvTable == null) && ParentScope is Scope scope)
            {
                TvTable = scope.TvTable;
            }
            else
            {
                TvTable = tvTable;
            }
        }

        public bool TryGetType(string typeName, out INamedType type)
        {
            if (TypeDefinitions.TryGetValue(typeName, out type))
            {
                return true;
            }

            if (ParentScope != null)
            {
                return ParentScope.TryGetType(typeName, out type);
            }

            return false;
        }

        public bool TryGetTypeForDefnName(string defnName, out IType type)
        {
            if (FuncDefnTypes.TryGetValue(defnName, out type))
            {
                return true;
            }

            if (ParentScope != null)
            {
                return ParentScope.TryGetTypeForDefnName(defnName, out type);
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

            return ParentScope.IsSubtypeOf(subtype, supertype);
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

            ParentScope.PrintTypeHeirarchy();
        }

        public bool VerifySymbols(params string[] symbols)
        {
            foreach (string s in symbols)
            {
                if (!FuncDefinitions.ContainsKey(s))
                {
                    return ParentScope.VerifySymbols(s);
                }
            }

            return true;
        }
    }
}
