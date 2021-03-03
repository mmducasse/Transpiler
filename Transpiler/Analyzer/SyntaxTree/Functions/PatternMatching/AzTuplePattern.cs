using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzTuplePattern(IReadOnlyList<IAzPattern> Elements,
                                 CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzTuplePattern Analyze(Scope scope,
                                             NameProvider provider,
                                             PsTuplePattern psTupPat)
        {
            var elements = psTupPat.Elements.Select(e => IAzPattern.Analyze(scope, provider, e)).ToList();

            return new(elements, psTupPat.Position);
        }

        public ConstraintSet Constrain()
        {
            var cs = new ConstraintSet();
            List<IAzTypeExpn> elementTypes = new();
            for (int i = 0; i < Elements.Count; i++)
            {
                var c = Elements[i].Constrain();
                elementTypes.Add(Elements[i].Type);

                cs = IConstraintSet.Union(cs, c);
            }

            var tupleType = new AzTypeTupleExpn(elementTypes, Null);
            var ctup = new Constraint(Type, tupleType, Position);

            return IConstraintSet.Union(cs, ctup);
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
            var es = Elements.Select(v => v.Print(i)).Separate(", ");
            return string.Format("{0}", es);
        }

        public override string ToString() => Print(0);
    }
}
