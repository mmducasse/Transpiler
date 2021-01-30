namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           IType Type);

    public static class OperatorUtil
    {
        public static Operator Function2(string name,
                                         INamedType type) =>
            Function2(name, type, type, type);

        public static Operator Function2(string name,
                                         INamedType arg1,
                                         INamedType arg2,
                                         INamedType ret)
        {
            var type = LambdaType.Make(arg1, arg2, ret);
            return new Operator(name, type);
        }
    }
}
