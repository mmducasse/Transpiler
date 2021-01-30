using System.Collections.Generic;

namespace Transpiler
{
    public interface ICompositeType : INamedType
    {
        IReadOnlyList<string> Elements { get; }
    }
}
