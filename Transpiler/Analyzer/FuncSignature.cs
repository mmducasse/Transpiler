namespace Transpiler.Analysis
{
    public record FuncSignature(string Name, IType Type) : IAzFuncDefn
    {
        public eFixity Fixity => eFixity.Prefix;

        public CodePosition Position => CodePosition.Null;

        public string Print(int i)
        {
            return string.Format("{0} :: {1}", Name, Type.Print(terse: true));
        }
    }
}
