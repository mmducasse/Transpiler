namespace Transpiler.Generate
{
    /// <summary>
    /// Syntax Tree node that was produced by the analyzer.
    /// </summary>
    public interface IGnNode
    {
        string Generate(int i, NameProvider names, ref string s);
    }
}
