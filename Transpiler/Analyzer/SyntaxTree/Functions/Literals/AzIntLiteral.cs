using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzIntLiteral(string Value,
                               CodePosition Position) : IAzLiteralExpn
    {
        public IAzDataTypeDefn CertainType => CoreTypes.Instance.Int;

        public static AzIntLiteral Analyze(Scope scope,
                                           PsIntLiteral node)
        {
            return new(node.Value, node.Position);
        }

        public string Print(int indent)
        {
            return Value.ToString();
        }
    }
}
