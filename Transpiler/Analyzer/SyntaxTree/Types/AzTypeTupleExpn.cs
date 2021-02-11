using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzTypeTupleExpn(IReadOnlyList<IAzTypeExpn> Elements,
                                  CodePosition Position) : IAzTypeExpn
    {
        public bool IsSolved => Elements.Where(e => !e.IsSolved).Count() == 0;

        public static AzTypeTupleExpn Analyze(Scope scope,
                                              PsTypeTupleExpn node)
        {
            var elements = node.Elements.Select(m => IAzTypeExpn.Analyze(scope, m)).ToList();

            return new(elements, node.Position);
        }

        public string Print(int i)
        {
            if (Elements.Count == 0)
            {
                return "()";
            }

            string elements = Elements.Select(m => m.Print(i)).Separate(", ");
            return string.Format("{0}", elements);
        }
    }
}
