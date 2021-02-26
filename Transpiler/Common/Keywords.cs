using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public static class Keywords
    {
        public const string If = "if";
        public const string Then = "then";
        public const string Else = "else";
        public const string Match = "match";
        public const string Type = "type";
        public const string TypeInstance = "inst";

        public static readonly string[] ReservedWords = new[]
        {
            If,
            Then,
            Else,
            Match,
            //Type,
            //Inst,
        };
    }
}
