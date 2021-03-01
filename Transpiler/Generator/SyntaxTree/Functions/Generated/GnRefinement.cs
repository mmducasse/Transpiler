using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public static class GnRefinement
    {
        public static string Generate(Refinement refinement)
        {
            return "d" + refinement.ClassType.Name + refinement.TypeVar.Name;
        }
    }
}
