namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           IType Type,
                           eFixity Fixity) : IAzFuncDefn
    {
        public CodePosition Position => CodePosition.Null;

        public string Print(int i)
        {
            return Name;
        }
    }

    public static class OperatorUtil
    {
        public static Operator Function2(string name,
                                         INamedType type,
                                         eFixity fixity = eFixity.Infix) =>
            Function2(name, type, type, type, fixity);

        public static Operator Function2(string name,
                                         INamedType arg1,
                                         INamedType arg2,
                                         INamedType ret,
                                         eFixity fixity = eFixity.Infix)
        {
            var type = FunType.Make(arg1, arg2, ret);
            return new Operator(name, type, fixity);
        }
    }
}
