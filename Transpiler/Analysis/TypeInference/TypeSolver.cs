using System;
using System.Collections.Generic;

namespace Transpiler
{
    public static class TypeSolver
    {
        public static void SolveFunctions(Scope scope)
        {
            foreach (var fn in scope.FuncDefinitions.Values)
            {
                var tvTable = new TvTable();
                var constraints = FuncDefnNode.Constrain(tvTable, scope, fn as FuncDefnNode);
                var substitution = IType.Unify(scope, constraints);

                Console.WriteLine("Before Solve:");
                tvTable.Print();

                Console.WriteLine("After Solve:");
                tvTable.Print(substitution);

                var tv = tvTable.GetTypeOf(fn);
                var type = IType.Substitute(tv, substitution);

                scope.FuncDefnTypes[fn.Name] = type;
            }
        }

        public static InterpreterException Error(string message, IAstNode node)
        {
            return new InterpreterException(eInterpreterStage.Analyzer, message);
        }
    }
}
