using System;
using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public interface IConstraints
    {
        public static ConstraintSet Union(params IConstraints[] cs)
        {
            var set = new ConstraintSet();
            foreach (var c in cs)
            {
                if (c is Constraint constraint)
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

    public record Constraint(IAzTypeExpn A, IAzTypeExpn B, IAzNode TEMP_Node) : IConstraints
    {
        public Constraint Substitute(Substitution sub)
        {
            return new Constraint(IAzTypeExpn.Substitute(A, sub),
                                  IAzTypeExpn.Substitute(B, sub),
                                  TEMP_Node);
        }
    }

    public class ConstraintSet : IConstraints
    {
        private HashSet<Constraint> mHashSet = new();

        public bool IsEmpty => mHashSet.Count == 0;

        public (Constraint, ConstraintSet) Next
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

        public ConstraintSet(HashSet<Constraint> hashSet)
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

        public void Add(Constraint c)
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
                s += string.Format("{0} = {1}\n", c.A.Print(0), c.B.Print(0));
                s += string.Format("{0}\n\n", c.TEMP_Node.Print(0));
            }

            return s;
        }

        public override string ToString()
        {
            string s = "";

            foreach (var c in mHashSet)
            {
                s += string.Format("{0} = {1}    ", c.A.Print(0), c.B.Print(0));
            }

            return s;
        }
    }
}
