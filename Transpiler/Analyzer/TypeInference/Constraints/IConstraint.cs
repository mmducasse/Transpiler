﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Analysis
{
    public interface IConstraintSet
    {
        public static ConstraintSet Union(params IConstraintSet[] cs)
        {
            var set = new ConstraintSet();
            foreach (var c in cs)
            {
                if (c is IConstraint constraint)
                {
                    set.Add(constraint);
                }
                else if (c is ConstraintSet constraintSet)
                {
                    set.UnionWith(constraintSet);
                }
            }

            return set;
        }
    }

    public interface IConstraint : IConstraintSet
    {
        CodePosition Position { get; }

        IConstraint Substitute(Substitution sub);

        public static Substitution Unify(IScope scope,
                                         ConstraintSet set,
                                         TvProvider tvProvider)
        {
            if (set.IsEmpty)
            {
                return new Substitution();
            }

            var (c, cs) = set.Next;

            return c switch
            {
                Constraint eqc => Constraint.Unify(scope, eqc, cs, tvProvider),
                _ => throw Analyzer.Error("Type inference failed.", c.Position),
            };
        }

        protected static bool Contains(IAzTypeExpn t, TypeVariable tv)
        {
            return t switch
            {
                TypeVariable tv2 => tv.Equals(tv2),
                AzTypeLambdaExpn lam => Contains(lam.Input, tv) || Contains(lam.Output, tv),
                // Todo: Add tuple case.
                AzTypeCtorExpn ctor => ctor.Contains(tv),
                _ => false,
            };
        }
    }
}
