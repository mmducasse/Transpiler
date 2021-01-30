using System.Collections.Generic;

namespace Transpiler
{
    public interface IScope
    {
        bool TryGetType(string typeName, out INamedType type);

        bool TryGetTypeForDefnName(string symbol, out IType type);

        //bool TryGetDefinition(string symbol, out IFuncDefnNode node);

        //bool TryGetTypeForDefn(IFuncDefnNode defn, out IType type);

        bool VerifySymbols(params string[] symbols);
    }

    public class Scope : IScope
    {
        public IScope ParentScope { get; }

        public Dictionary<string, INamedType> Types { get; } = new();

        public Dictionary<string, IFuncDefnNode> Definitions { get; } = new();

        public Dictionary<string, IType> DefnTypes { get; } = new();

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
            if (Types.TryGetValue(typeName, out type))
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
            if (DefnTypes.TryGetValue(defnName, out type))
            {
                return true;
            }

            if (ParentScope != null)
            {
                return ParentScope.TryGetTypeForDefnName(defnName, out type);
            }

            return false;
        }

        public bool VerifySymbols(params string[] symbols)
        {
            foreach (string s in symbols)
            {
                if (!Definitions.ContainsKey(s))
                {
                    return ParentScope.VerifySymbols(s);
                }
            }

            return true;
        }
    }
}
