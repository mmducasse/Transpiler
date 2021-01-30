namespace Transpiler
{
    public record CodePosition(Module Module,
                               int Line,
                               int Column)
    {
        public CodePosition NextColumn => new(Module, Line, Column + 1);
        public CodePosition NextLine => new(Module, Line + 1, 0);

        public static CodePosition Zero(Module module) => new(module, 0, 0);

        //public override string ToString() => string.Format("{0} ({1}, {2})", Module.Name, Row, Col);
    }
}
