using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTypeLambdaExpn(IAzTypeExpn Input,
                                   IAzTypeExpn Output,
                                   CodePosition Position) : IAzTypeExpn
    {
        public bool IsSolved => Input.IsSolved && Output.IsSolved;

        public static AzTypeLambdaExpn Make(params IAzTypeExpn[] elements)
        {
            var p = CodePosition.Null;

            return elements.Length switch
            {
                < 2 => throw new InvalidOperationException(),
                2 => new AzTypeLambdaExpn(elements[0], elements[1], p),
                _ => new AzTypeLambdaExpn(elements[0], Make(elements[1..]), p),
            };
        }

        public static AzTypeLambdaExpn Analyze(Scope scope,
                                               PsTypeLambdaExpn node)
        {
            var input = IAzTypeExpn.Analyze(scope, node.Input);
            var output = IAzTypeExpn.Analyze(scope, node.Output);

            return new(input, output, node.Position);
        }

        public static IAzTypeExpn Substitute(AzTypeLambdaExpn lamType, Substitution sub)
        {
            return new AzTypeLambdaExpn(IAzTypeExpn.Substitute(lamType.Input, sub),
                                        IAzTypeExpn.Substitute(lamType.Output, sub),
                                        lamType.Position);
        }
        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Input.Print(i), Output.Print(i));
        }
    }
}
