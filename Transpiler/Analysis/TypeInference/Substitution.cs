using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transpiler
{
    public record Substitution(IType OldType, IType NewType);
}
