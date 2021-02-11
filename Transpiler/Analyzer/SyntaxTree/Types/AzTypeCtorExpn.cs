using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTypeCtorExpn(IAzTypeDefn TypeDefn,
                                 IReadOnlyList<IAzTypeExpn> Arguments,
                                 CodePosition Position) : IAzTypeExpn
    {
        public bool IsSolved => Arguments.Where(a => !a.IsSolved).Count() == 0;

        public static AzTypeCtorExpn Analyze(Scope scope,
                                             PsTypeArbExpn node)
        {
            if (!scope.TryGetNamedType(node.TypeName, out var type))
            {
                throw Analyzer.Error("Undefined type " + node.TypeName, node.Position);
            }

            List<IAzTypeExpn> args = new();
            foreach (var expn in node.Children)
            {
                args.Add(IAzTypeExpn.Analyze(scope, expn));
            }

            return new(type, args, node.Position);
        }

        public string Print(int indent)
        {
            string args = Arguments.Select(a => a.Print(0)).Separate(" ", prepend: " ");
            return string.Format("{0}{1}", TypeDefn.Name, args);
        }
    }
}
