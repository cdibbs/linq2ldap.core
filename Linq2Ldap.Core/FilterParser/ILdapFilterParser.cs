using System;
using System.Linq.Expressions;
using Linq2Ldap.Core.FilterParser.Models;
using Linq2Ldap.Core.Models;

namespace Linq2Ldap.Core.FilterParser {
    public interface ILdapFilterParser {
        /// <summary>
        /// Parses an LDAP filter from the given string into an Expression&lt;Func&lt;T,bool&gt;&gt;.
        /// </summary>
        /// <typeparam name="T">A type implementing IEntry whose indexer to use for attribute references. E.g., m["email"].</typeparam>
        /// <param name="filter">An LDAP filter string (RFC 1960).</param>
        /// <param name="modelName">An optional model variable name to use in the resulting Expression. Defaults to "m".</param>
        /// <returns>A predicate Expression representing the input filter and accepting one parameter of type T.</returns>
        Expression<Func<T, bool>> Parse<T>(
            string filter,
            string modelName = "m"
        ) where T: IEntry;
    }
}