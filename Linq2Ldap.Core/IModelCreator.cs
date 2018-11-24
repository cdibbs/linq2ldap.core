using Linq2Ldap.Core.Models;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core {
    public interface IModelCreator {
        /// <summary>
        /// Creates a new instance of an LDAP Entry model and populates its fields.
        /// </summary>
        /// <param name="entryProps">A proxy dictionary of the LDAP entry's fields.</param>
        /// <param name="dn">DistinguishedName field of this entry.</param>
        /// <typeparam name="T">The destination IEntry model type.</typeparam>
        /// <returns>A populated instance of T.</returns>
        T Create<T>(EntryAttributeDictionary entryProps, string dn = null)
            where T: IEntry, new();
    }
}