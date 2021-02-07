﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public static class TvUtils
    {
        public static ISet<TypeVariable> GetTvs(this IType type)
        {
            return type switch
            {
                TypeVariable typeVar => new HashSet<TypeVariable> { typeVar },
                FunType funType => GetTvs(funType.Input).Union(GetTvs(funType.Output)).ToHashSet(),
                _ => new HashSet<TypeVariable>(),
            };
        }

        public static IType MadeUnique(this IType type)
        {
            var tvs = type.GetTvs().ToList();
            var newTvs = tvs.Select(tv => tv.MadeUnique).ToList();

            var subs = new List<Substitution>();
            for (int i = 0; i < tvs.Count; i++)
            {
                var sub = new Substitution(tvs[i], newTvs[i]);
                subs.Add(sub);
            }
            var substitution = new Substitution(subs.ToArray());

            return IType.Substitute(type, substitution);
        }

        public static TypeVariable Unify(IScope scope,
                                         TypeVariable tva,
                                         TypeVariable tvb)
        {
            var ra = tva.Refinements;
            var rb = tvb.Refinements;
            var rc = ra.Union(rb).ToArray();
            return TypeVariable.NextR(rc);
        }
    }
}
