namespace Transpiler
{
    public record PrimitiveType(string Name) : INamedType
    {
        public bool IsSolved => true;

        public string Print() => Name;
    }
}
