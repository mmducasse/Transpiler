using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Extensions;
using static Transpiler.Parse.ParserUtils;
using static Transpiler.Parser;

namespace Transpiler
{
    public record AzMatchExpn(IAzFuncExpn Argument,
                              IReadOnlyList<MatchCaseNode> Cases) : IAzFuncExpn
    {
        public string Print(int i)
        {
            string s = string.Format("match {0}\n", Argument.Print(i));
            foreach (var c in Cases)
            {
                s += string.Format("{0}| {1}\n", Indent(i + 1), c.Print(i + 1));
            }

            return s;
        }
    }
}
