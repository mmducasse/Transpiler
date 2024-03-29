﻿// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

namespace Transpiler.Parse
{
    /// <summary>
    /// A type definition.
    /// </summary>
    public interface IPsTypeDefn : IPsDefn
    {
        public static bool Parse(ref TokenQueue queue, bool allowClasses, out IPsTypeDefn node)
        {
            node = null;
            var q = queue;

            if (allowClasses &&
                PsClassTypeDefn.Parse(ref q, out var classDefn)) { node = classDefn; }
            else if (PsUnionTypeDefn.Parse(ref q, out var uniDefn)) { node = uniDefn; }
            else if (PsDataTypeDefn.Parse(ref q, out var dataDefn)) { node = dataDefn; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
