using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzRealLiteral(double Value,
                                CodePosition Position) : IAzLiteralExpn<double>
    {
        public IAzTypeDefn CertainType => CoreTypes.Instance.Real;

        public static AzRealLiteral Analyze(Scope scope,
                                            PsRealLiteral node)
        {
            return new(node.Value, node.Position);
        }

        public string Print(int indent)
        {
            return ((int)Value == Value)
                ? string.Format("{0:0.0}", Value)
                : Value.ToString();
        }
    }
}
