using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzParam(string Name,
                          CodePosition Position) : IAzPattern, IAzFuncDefn
    {
        public IAzTypeExpn ExplicitType => null;

        public static AzParam Analyze(Scope scope,
                                      PsParam node)
        {
            var azParam = new AzParam(node.Name, node.Position);

            scope.FuncDefinitions[azParam.Name] = azParam;

            return azParam;
        }

        public static bool Solve(Scope scope,
                                 AzParam node)
        {
            //var tfx = table.GetTypeOf(node);
            //var tf = table.GetTypeOf(node.Function);
            //var tx = table.GetTypeOf(node.Argument);

            return false;
        }

        public string Print(int i) => Name;

        public override string ToString() => Print(0);
    }
}
