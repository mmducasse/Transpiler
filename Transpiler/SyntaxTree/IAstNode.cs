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
                ParamNode => node.Print(0),
                AppNode => "app",
                FuncDefnNode fnDefnNode => fnDefnNode.Name + " =",
                IfNode => "if ",
                LambdaNode => "lam",
                ILiteralNode => node.Print(0),
                MatchNode => "mat",
                SymbolNode symNode => node.Print(0),
                ScopedFuncExpnNode => "scp",
                _ => "???"
            };
        }
    }
}
