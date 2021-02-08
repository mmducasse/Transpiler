using System;
using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public record MatchCaseNode(IAzPattern Pattern,
                                IFuncExpnNode Expression)
    {

        public string Print(int i)
        {
            return string.Format("{0} -> {1}", Pattern.Print(i), Expression.Print(i + 1));
        }
    }
}
