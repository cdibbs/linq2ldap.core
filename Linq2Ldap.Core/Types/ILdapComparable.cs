using Linq2Ldap.Core.Types;

namespace Linq2Ldap.Core.Types {
    public interface ILdapComparable<T> where T: System.IComparable
    {
        /// <summary>
        /// Facilitates comparison overloads with many-valued types. See the IntList
        /// implementation for what to return.
        /// </summary>
        /// <param name="b">The IComparable to compare with the many-valued type.</param>
        /// <returns>An IntList containing comparison results.</returns>
        IntList CompareTo(T b);
    }
}