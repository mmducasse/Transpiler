﻿using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Analysis
{
    public class Substitution
    {
        public IReadOnlyDictionary<TypeVariable, IAzTypeExpn> TypeSubstitutions => mTypeSubstitutions;
        private Dictionary<TypeVariable, IAzTypeExpn> mTypeSubstitutions = new();

        public Substitution(TypeVariable tv, IAzTypeExpn newType)
        {
            if (!IAzTypeExpn.Equate(tv, newType))
            {
                mTypeSubstitutions[tv] = newType;
            }
        }

        public Substitution(params Substitution[] substitutions)
        {
            foreach (var s in substitutions)
            {
                foreach (var kvp in s.TypeSubstitutions)
                {
                    if (!mTypeSubstitutions.ContainsKey(kvp.Key))
                    {
                        mTypeSubstitutions.Add(kvp.Key, kvp.Value);
                    }
                }
            }
        }

        public void Add(TypeVariable tv, IAzTypeExpn newType)
        {
            mTypeSubstitutions[tv] = newType;
        }

        public void Add(Substitution otherSubstitution)
        {
            foreach (var kvp in otherSubstitution.TypeSubstitutions)
            {
                if (!mTypeSubstitutions.ContainsKey(kvp.Key))
                {
                    mTypeSubstitutions.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public string Print()
        {
            return TypeSubstitutions.
                Select(kvp => string.Format("{{{0} / {1}}}\n", kvp.Value.Print(0), kvp.Key.Print())).Separate(", ");
        }

        public override string ToString() => Print();
    }
}
