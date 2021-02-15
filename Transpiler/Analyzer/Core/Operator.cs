namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           IAzTypeExpn Type,
                           eFixity Fixity = eFixity.Infix) : IAzFuncDefn
    {
        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn ExplicitType => Type;

        public string Print(int i) => Name;

        public override string ToString() => Print(0);
    }

    public static class OperatorUtil
    {
        public static Operator Function2(string name,
                                         IAzDataTypeDefn type,
                                         eFixity fixity = eFixity.Infix) =>
            Function2(name, type, type, type, fixity);

        public static Operator Function2(string name,
                                         IAzDataTypeDefn arg1,
                                         IAzDataTypeDefn arg2,
                                         IAzDataTypeDefn ret,
                                         eFixity fixity = eFixity.Infix)
        {
            var type = AzTypeLambdaExpn.Make(arg1.ToCtor(),
                                             arg2.ToCtor(),
                                             ret.ToCtor());
            return new Operator(name, type, fixity);
        }
    }
}
