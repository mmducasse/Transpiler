namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           IAzTypeExpn Type,
                           eFixity Fixity) : IAzFuncDefn
    {
        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn ExplicitType => Type;

        public string Print(int i)
        {
            return Name;
        }
    }

    public static class OperatorUtil
    {
        public static Operator Function2(string name,
                                         IAzTypeDefn type,
                                         eFixity fixity = eFixity.Infix) =>
            Function2(name, type, type, type, fixity);

        public static Operator Function2(string name,
                                         IAzTypeDefn arg1,
                                         IAzTypeDefn arg2,
                                         IAzTypeDefn ret,
                                         eFixity fixity = eFixity.Infix)
        {
            var type = AzTypeLambdaExpn.Make(arg1.ToSym(),
                                             arg2.ToSym(),
                                             ret.ToSym());
            return new Operator(name, type, fixity);
        }
    }
}
