using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public class TvProvider
    {
        private int mNextIndex = 0;

        public TypeVariable Next => TypeVariable.Simple(mNextIndex++);

        public TypeVariable NextR(params AzClassTypeDefn[] refinements) =>
            new(mNextIndex++, refinements);

        public TypeVariable MadeUnique(TypeVariable tv) => tv with { Id = mNextIndex++ };
    }

    public static class TypeVariables
    {
        public static void Reset()
        {
            Provider = new();
        }

        public static TvProvider Provider { get; private set; } = new();

        public static TypeVariable Next => Provider.Next;
    }
}
