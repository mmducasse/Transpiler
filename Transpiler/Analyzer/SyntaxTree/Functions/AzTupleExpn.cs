using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzTupleExpn(IReadOnlyList<IAzFuncExpn> Elements,
                              IAzTypeExpn Type,
                              CodePosition Position) : IAzFuncExpn
    {
        public static AzTupleExpn Analyze(Scope scope,
                                          NameProvider names,
                                          TvProvider tvs,
                                          PsTupleExpn psTupExpn)
        {
            var elements = psTupExpn.Elements.Select(e => IAzFuncExpn.Analyze(scope, names, tvs, e)).ToList();
            return new(elements, tvs.Next, psTupExpn.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            ConstraintSet cs = new();
            foreach (var e in Elements)
            {
                var cse = e.Constrain(provider, scope);
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

        public IAzFuncExpn SubstituteType(Substitution s)
        {
            return new AzTupleExpn(Elements.Select(e => e.SubstituteType(s)).ToList(),
                                   Type.Substitute(s),
                                   Position);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var elementNodes = Elements.SelectMany(e => e.GetSubnodes()).ToList();
            return this.ToArr().Concat(elementNodes).ToList();
        }

        public string Print(int i)
        {
            return Elements.Select(e => e.Print(i)).Separate(", ");
        }

        public override string ToString() => Print(0);
    }
}
