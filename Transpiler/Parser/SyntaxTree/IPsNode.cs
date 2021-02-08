namespace Transpiler.Parse
{
    /// <summary>
    /// Syntax Tree node that was produced by the parser.
    /// </summary>
    public interface IPsNode
    {
        CodePosition Position { get; }

        string Print(int i);
    }

    //public static class AstNodeUtil
    //{
    //    public static string Abbr(this IPsNode node)
    //    {
    //        return node switch
    //        {
    //            PsParam => node.Print(0),
    //            //AppNode => "app",
    //            PsFuncDefn fnDefnNode => fnDefnNode.PrintSignature(0) + " =",
    //            PsIfExpn => "if ",
    //            PsLambdaExpn => "lam",
    //            IPsLiteralExpn => node.Print(0),
    //            PsMatch => "mat",
    //            PsSymbolExpn symNode => node.Print(0),
    //            PsScopedFuncExpn => "scp",
    //            _ => "???"
    //        };
    //    }
    //}
}
