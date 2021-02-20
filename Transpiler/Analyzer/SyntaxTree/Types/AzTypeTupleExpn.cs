using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTypeTupleExpn(IReadOnlyList<IAzTypeExpn> Elements,
                                  CodePosition Position) : IAzTypeExpn
    {
        public ISet<TypeVariable> GetTypeVars()
        {
            HashSet<TypeVariable> tvs = new();
            foreach (var e in Elements)
            {
                tvs.UnionWith(e.GetTypeVars());
            }
            return tvs;
        }

        public static bool Equate(AzTypeTupleExpn a, AzTypeTupleExpn b)
        {
            if (a.Elements.Count != b.Elements.Count) { return false; }

            for (int i = 0; i < a.Elements.Count; i++)
            {
                if (false == IAzTypeExpn.Equate(a.Elements[i], b.Elements[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static AzTypeTupleExpn Analyze(Scope scope,
                                              PsTypeTupleExpn node)
        {
            var elements = node.Elements.Select(m => IAzTypeExpn.Analyze(scope, m)).ToList();

            return new(elements, node.Position);
        }

        public IAzTypeExpn Substitute(Substitution substitution)
        {
            var newElements = Elements.Select(e => e.Substitute(substitution)).ToList();
            return this with { Elements = newElements };
        }

        public string Print(int i)
        {
            if (Elements.Count == 0)
            {
                return "()";
            }

            string elements = Elements.Select(m => m.Print(i)).Separate(", ");
            return string.Format("({0})", elements);
        }

        public override string ToString() => Print(0);
    }
}
