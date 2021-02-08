namespace Transpiler.Analysis
{
    public record PrimitiveType(string Name) : INamedType
    {
        public bool IsSolved => true;

        public string Print(bool terse = true) => Name;
    }
}
