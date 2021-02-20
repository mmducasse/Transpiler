using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public static class TvUtils
    {
        public static Substitution UniqueTvSubstitution(this IAzTypeExpn type, TvProvider provider)
        {
            var tvs = type.GetTypeVars().ToList();
            var newTvs = tvs.Select(tv => provider.MadeUnique(tv)).ToList();

            var subs = new List<Substitution>();
            for (int i = 0; i < tvs.Count; i++)
            {
                var sub = new Substitution(tvs[i], newTvs[i]);
                subs.Add(sub);
            }
            return new Substitution(subs.ToArray());
        }

        public static IAzTypeExpn WithUniqueTvs(this IAzTypeExpn type, TvProvider provider)
        {
            var substitution = UniqueTvSubstitution(type, provider);
            return type.Substitute(substitution);
        }

        public static TypeVariable Unify(IScope scope,
                                         TypeVariable tva,
                                         TypeVariable tvb,
                                         TvProvider provider)
        {
            var ra = tva.Refinements;
            var rb = tvb.Refinements;
            var rc = ra.Union(rb).ToArray();
            return provider.NextR(rc);
        }

        public static string PrintWithRefinements(this IAzTypeExpn type)
        {
            var tvs = type.GetTypeVars();

            List<string> refinements = new();
            foreach (var tv in tvs)
            {
                if (tv.HasRefinements)
                {
                    foreach (var r in tv.Refinements)
                    {
                        refinements.Add(string.Format("{0} {1}", r.Name, tv.Print()));
                    }
                }
            }

            if (refinements.Count == 0)
            {
                return type.Print(0);
            }

            return string.Format("{0} => {1}", refinements.Separate(", "), type.Print(0));
        }
    }
}
