using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public static class TypeSolver
    {
        public static IReadOnlyList<FuncDefnNode>
            SolveFunctions(Scope scope, IReadOnlyList<FuncDefnNode> funcDefns)
        {
            List<FuncDefnNode> newFns = new();
            foreach (var fn in funcDefns)
            {
                var constraints = FuncDefnNode.Constrain(scope, fn);
                //Console.WriteLine(constraints.Print());

                Console.WriteLine("Before Solve:");
                foreach (var (node, t) in scope.TvTable.NodeTypes)
                {
                    Console.Write("{0, 30} :: ", node.Print(0));
                    Console.WriteLine(t.Print());
                }


                var substitution = IType.Unify(constraints);
                Console.WriteLine(substitution.Print());

                Console.WriteLine("After Solve:");
                foreach (var (node, t) in scope.TvTable.NodeTypes)
                {
                    var solvedType = IType.Substitute(t, substitution);

                    Console.Write("{0, 30} :: ", node.Print(0));
                    Console.WriteLine(solvedType.Print());
                }

                var tv = scope.TvTable.GetTypeOf(fn);
                var type = IType.Substitute(tv, substitution);
            }

            return newFns;
        }
    }
}
