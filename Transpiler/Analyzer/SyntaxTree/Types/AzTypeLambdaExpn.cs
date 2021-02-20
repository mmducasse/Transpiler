using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTypeLambdaExpn(IAzTypeExpn Input,
                                   IAzTypeExpn Output,
                                   CodePosition Position) : IAzTypeExpn
    {
        public ISet<TypeVariable> GetTypeVars()
        {
            HashSet<TypeVariable> tvs = new();
            tvs.UnionWith(Input.GetTypeVars());
            tvs.UnionWith(Output.GetTypeVars());
            return tvs;
        }

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

        public IAzTypeExpn Substitute(Substitution substitution)
        {
            return this with { Input = Input.Substitute(substitution),
                               Output = Output.Substitute(substitution) };
        }

        public string Print(int i)
        {
            if (Input is AzTypeLambdaExpn)
            {
                return string.Format("({0}) -> {1}", Input.Print(i), Output.Print(i));
            }
            return string.Format("{0} -> {1}", Input.Print(i), Output.Print(i));
        }

        public override string ToString() => this.PrintWithRefinements();
    }
}
