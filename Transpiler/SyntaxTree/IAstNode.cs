namespace Transpiler
{
    public interface IAstNode
    {
        string Print(int indent);
    }

    public static class AstNodeUtil
    {
        public static string Abbr(this IAstNode node)
        {
            return node switch
            {
                ArgNode => "arg",
                AppNode => "app",
                FuncDefnNode => "def",
                IfNode => "if ",
                LambdaNode => "lam",
                ILiteralNode => "lit",
                MatchNode => "mat",
                SymbolNode => "sym",
                ScopedFuncExpnNode => "scp",
                _ => "???"
            };
        }
    }
}
