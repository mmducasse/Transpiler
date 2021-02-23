using System;
using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public class ConstraintSet : IConstraintSet
    {
        private HashSet<IConstraint> mHashSet = new();

        public bool IsEmpty => mHashSet.Count == 0;

        public (IConstraint, ConstraintSet) Next
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException();
                }

                var list = mHashSet.ToArray();
                var c = list.First();
                var cs = new ConstraintSet(list[1..].ToHashSet());

                return (c, cs);
            }
        }

        public ConstraintSet()
        {
        }

        public ConstraintSet(HashSet<IConstraint> hashSet)
        {
            mHashSet = hashSet;
        }

        public static ConstraintSet Empty => new ConstraintSet();

        public ConstraintSet Substitute(Substitution sub)
        {
            var newCs = new ConstraintSet();

            foreach (var c in mHashSet)
            {
                newCs.Add(c.Substitute(sub));
            }

            return newCs;
        }

        public void Add(IConstraint c)
        {
            mHashSet.Add(c);
        }

        public void UnionWith(ConstraintSet cs)
        {
            mHashSet.UnionWith(cs.mHashSet);
        }

        public string Print()
        {
            string s = "";

            foreach (var c in mHashSet)
            {
                s += string.Format("{0}\n", c.ToString());
            }

            return s;
        }

        public override string ToString()
        {
            return mHashSet.Select(c => c).Separate(", ");
        }
    }
}
