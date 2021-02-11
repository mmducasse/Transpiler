namespace Transpiler.Analysis
{
    /// <summary>
    /// Syntax Tree node that was produced by the analyzer.
    /// </summary>
    public interface IAzNode
    {
        CodePosition Position { get; }

        string Print(int i);
    }
}
