using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzCharLiteral(string Value,
                                CodePosition Position) : IAzLiteralExpn
    {
        public IAzDataTypeDefn CertainType => CoreTypes.Instance.Char;

        public static AzCharLiteral Analyze(Scope scope,
                                            PsCharLiteral node)
        {
            return new(node.Value, node.Position);
        }

        public string Print(int indent)
        {
            return string.Format("'{0}'", Value);
        }

        public override string ToString() => Print(0);
    }
}
