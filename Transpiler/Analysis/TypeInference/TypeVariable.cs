namespace Transpiler
{
    public record TypeVariable(int Id) : IType
    {
        public bool IsSolved => false;

        private static int mTvIndex = 0;

        public static TypeVariable Next => new(mTvIndex++);

        public string Print() => string.Format("T{0}", Id);
    }
}
