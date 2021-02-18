using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler.Generate
{
    public class NameProvider
    {
        private int mIndex = 0;

        public string Next => ((char)('a' + (mIndex++))).ToString();
    }
}
