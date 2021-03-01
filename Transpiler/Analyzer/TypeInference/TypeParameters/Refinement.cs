namespace Transpiler.Analysis
{
    public record Refinement(AzClassTypeDefn ClassType,
                             TypeVariable TypeVar)
    {
        public string Print(int i)
        {
            return string.Format("{0} {1}", ClassType.Name, TypeVar.Print(i));
        }

        public override string ToString() => Print(0);
    }
}
