using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzTupleExpn(IReadOnlyList<IAzFuncExpn> Elements,
                              CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; set; }

        public static AzTupleExpn Analyze(Scope scope,
                                          PsTupleExpn node)
        {
            List<IAzFuncExpn> elements = new();
            foreach (var n in node.Elements)
            {
                var expn = IAzFuncExpn.Analyze(scope, n);
                elements.Add(expn);
            }

            return new(elements, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

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
            var ct = new Constraint(Type, tupType, this);

            return IConstraintSet.Union(ct, cs);
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
