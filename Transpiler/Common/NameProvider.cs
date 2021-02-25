using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public class NameProvider
    {
        private int mIndex = 0;

        public string Prefix { get; }

        public string Next => Prefix + (mIndex++);

        public NameProvider(string prefix)
        {
            Prefix = prefix;
        }
    }
}
