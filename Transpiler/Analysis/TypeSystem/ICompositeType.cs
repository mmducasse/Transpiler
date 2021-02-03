using System.Collections.Generic;

namespace Transpiler
{
    public interface ICompositeType : IType
    {
        public IType Left { get; }
        public IType Right { get; }
    }
}
