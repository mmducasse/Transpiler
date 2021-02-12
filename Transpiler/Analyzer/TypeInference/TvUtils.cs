using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public static class TvUtils
    {
        public static ISet<TypeVariable> GetTvs(this IAzTypeExpn type)
        {
            return type switch
            {
                TypeVariable typeVar => new HashSet<TypeVariable> { typeVar },
                AzTypeLambdaExpn funType => GetTvs(funType.Input).Union(GetTvs(funType.Output)).ToHashSet(),
                _ => new HashSet<TypeVariable>(),
            };
        }

        public static IAzTypeExpn WithUniqueTvs(this IAzTypeExpn type, TvProvider tvProvider)
        {
            var tvs = type.GetTvs().ToList();
            var newTvs = tvs.Select(tv => tvProvider.MadeUnique(tv)).ToList();

            var subs = new List<Substitution>();
            for (int i = 0; i < tvs.Count; i++)
            {
                var sub = new Substitution(tvs[i], newTvs[i]);
                subs.Add(sub);
            }
            var substitution = new Substitution(subs.ToArray());

            return IAzTypeExpn.Substitute(type, substitution);
        }

        public static TypeVariable Unify(IScope scope,
                                         TypeVariable tva,
                                         TypeVariable tvb,
                                         TvProvider tvProvider)
        {
            var ra = tva.Refinements;
            var rb = tvb.Refinements;
            var rc = ra.Union(rb).ToArray();
            return tvProvider.NextR(rc);
        }
    }
}
