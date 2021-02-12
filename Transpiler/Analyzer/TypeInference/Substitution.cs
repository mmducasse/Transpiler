﻿using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public class Substitution
    {
        public IReadOnlyDictionary<TypeVariable, IAzTypeExpn> TypeSubstitutions => mTypeSubstitutions;
        private Dictionary<TypeVariable, IAzTypeExpn> mTypeSubstitutions = new();

        public Substitution(TypeVariable tv, IAzTypeExpn newType)
        {
            mTypeSubstitutions[tv] = newType;
        }

        public Substitution(params Substitution[] substitutions)
        {
            foreach (var s in substitutions)
            {
                foreach (var kvp in s.TypeSubstitutions)
                {
                    mTypeSubstitutions.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public string Print()
        {
            string s = "";
            foreach (var kvp in TypeSubstitutions)
            {
                s += string.Format("{0} / {1}\n", kvp.Value.Print(0), kvp.Key.Print());
            }

            return s;
        }
    }
}