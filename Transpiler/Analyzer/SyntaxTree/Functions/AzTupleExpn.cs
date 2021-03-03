using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzTupleExpn(IReadOnlyList<IAzFuncExpn> Elements,
                              CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzTupleExpn Analyze(Scope scope,
                                          NameProvider names,
                                          PsTupleExpn psTupExpn)
        {
            var elements = psTupExpn.Elements.Select(e => IAzFuncExpn.Analyze(scope, names, e)).ToList();
            return new(elements, psTupExpn.Position);
        }

        public ConstraintSet Constrain()
        {
            ConstraintSet cs = new();
            foreach (var e in Elements)
            {
                var cse = e.Constrain();
                cs = IConstraintSet.Union(cs, cse);
            }

            List<IAzTypeExpn> tes = new();
            foreach (var e in Elements)
            {
                tes.Add(e.Type);
            }

            var tupType = new AzTypeTupleExpn(tes, Null);
            var ct = new Constraint(Type, tupType, Position);

            return IConstraintSet.Union(ct, cs);
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Elements.Foreach(e => e.Recurse(action));
            action(this);
        }

        public string Print(int i)
        {
            return Elements.Select(e => e.Print(i)).Separate(", ");
        }

        public override string ToString() => Print(0);
    }
}
