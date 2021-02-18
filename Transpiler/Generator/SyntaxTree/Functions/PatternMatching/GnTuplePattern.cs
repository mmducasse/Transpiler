//using System.Collections.Generic;
//using System.Linq;
//using Transpiler.Parse;

//namespace Transpiler.Analysis
//{
//    public record AzTuplePattern(IReadOnlyList<IAzPattern> Elements,
//                                 CodePosition Position) : IAzPattern
//    {
//        public static AzTuplePattern Analyze(Scope scope,
//                                              PsTuplePattern node)
//        {
//            var elements = node.Elements.Select(e => IAzPattern.Analyze(scope, e)).ToList();

//            return new(elements, node.Position);
//        }

//        public static ConstraintSet Constrain(TvTable tvTable,
//                                              Scope scope,
//                                              AzTuplePattern node)
//        {
//            var cs = new ConstraintSet();
//            List<IAzTypeExpn> elementTypes = new();
//            for (int i = 0; i < node.Elements.Count; i++)
//            {
//                var c = IAzPattern.Constrain(tvTable, scope, node.Elements[i]);

//                var tv = tvTable.AddNode(scope, node.Elements[i]);
//                elementTypes.Add(tv);

//                cs = IConstraintSet.Union(cs, c);
//            }

//            var tupleType = new AzTypeTupleExpn(elementTypes, CodePosition.Null);
//            var pattType = tvTable.GetTypeOf(node);
//            var ctup = new Constraint(pattType, tupleType, node);

//            return IConstraintSet.Union(cs, ctup);
//        }

//        public string Print(int i)
//        {
//            var es = Elements.Select(v => v.Print(i)).Separate(", ");
//            return string.Format("{0}", es);
//        }

//        public override string ToString() => Print(0);
//    }
//}
