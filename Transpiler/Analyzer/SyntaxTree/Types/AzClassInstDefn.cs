using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Analysis
{
    public record AzClassInstDefn(AzClassTypeDefn Class,
                                  IAzTypeDefn Implementor,
                                  IReadOnlyList<IAzFuncDefn> Functions,
                                  CodePosition Position) : IAzDefn
    {
        public bool IsSolved => true;

        public string Name => "instance";

        //public string Print(bool terse = true)
        //{
        //    string s = string.Format("impl {0} {1}", Class.Print(0), Implementor.Print(0));
        //    if (!terse)
        //    {
        //        s += "::\n";
        //    }

        //    return s;
        //}

        public string Print(int indent)
        {
            throw new NotImplementedException();
        }
    }
}
