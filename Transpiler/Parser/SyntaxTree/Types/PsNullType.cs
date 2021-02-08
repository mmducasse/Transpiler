namespace Transpiler.Parse
{
    public record PsNullType(CodePosition Position) : IPsTypeExpn
    {
        public string Print(int indent)
        {
            return "()";
        }
    }
}
