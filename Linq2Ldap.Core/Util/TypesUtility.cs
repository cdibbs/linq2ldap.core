using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.Util
{
    internal class TypesUtility
    {
        internal static bool CanMakeGenericTypeAssignableFrom(Type t, Type[] genArgs, Type from)
        {
            // Per Skeet, here: https://stackoverflow.com/a/4864565/2356600
            try
            {
                return t.MakeGenericType(genArgs).IsAssignableFrom(from);
            }
            catch
            {
                return false;
            }
        }
    }
}
