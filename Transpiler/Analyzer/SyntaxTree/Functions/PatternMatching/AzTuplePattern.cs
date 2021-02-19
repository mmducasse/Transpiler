using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzTuplePattern(IReadOnlyList<IAzPattern> Elements,
                                 CodePosition Position) : IAzPattern
    {
        public IAzTypeExpn Type { get; set; }

        public static AzTuplePattern Analyze(Scope scope,
                                              PsTuplePattern node)
        {
            var elements = node.Elements.Select(e => IAzPattern.Analyze(scope, e)).ToList();

            return new(elements, node.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            Type = provider.Next;

            var cs = new ConstraintSet();
            List<IAzTypeExpn> elementTypes = new();
            for (int i = 0; i < Elements.Count; i++)
            {
                var c = Elements[i].Constrain(provider, scope);
                elementTypes.Add(Elements[i].Type);

                cs = IConstraintSet.Union(cs, c);
            }

            var tupleType = new AzTypeTupleExpn(elementTypes, Null);
            var ctup = new Constraint(Type, tupleType, this);

            return IConstraintSet.Union(cs, ctup);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            var elementNodes = Elements.SelectMany(e => e.GetSubnodes()).ToList();
            return this.ToArr().Concat(elementNodes).ToList();
        }

        public string Print(int i)
        {
            var es = Elements.Select(v => v.Print(i)).Separate(", ");
            return string.Format("{0}", es);
        }

        public override string ToString() => Print(0);
    }
}
