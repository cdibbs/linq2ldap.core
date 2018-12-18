using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Models
{
    /// <summary>
    /// An interface for providing your own DirectoryEntry concrete implementations. Classes
    /// implementing this can be used with LdapFilterParser and LdapFilterCompiler.
    /// </summary>
    public interface IEntry
    {
        string DistinguishedName { get; set; }

        EntryAttributeDictionary Attributes { get; set; }

        bool Has(string attrName);

        AttributeValueList this[string key] { get; }

        /// <summary>
        /// Extended match filters.
        /// </summary>
        /// <param name="rule">The matching rule to use.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="isDnAttr">Whether this is a dn filter.</param>
        /// <returns>An attribute value list for which only the equals operator applies.</returns>
        EqualsOnlyAttributeValueList this[string attr, Rule rule, bool isDn] { get; }

        /// <summary>
        /// Extended match filter to add a match rule.
        /// </summary>
        /// <param name="rule">The matching rule to use.</param>
        /// <param name="key">The attribute key.</param>
        /// <returns>An attribute value list for which only the equals operator applies.</returns>
        EqualsOnlyAttributeValueList this[string attr, Rule rule] { get; }


        /// <summary>
        /// Extended match filters for distinguishedNames.
        /// </summary>
        /// <param name="key">The dn attribute.</param>
        /// <param name="isDnAttr">That this is a dn filter.</param>
        /// <returns>An attribute value list for which only the equals operator applies.</returns>
        EqualsOnlyAttributeValueList this[string attr, bool isDnAttr] { get; }

        /// <summary>
        /// Extended match filters for distinguishedNames with a match rule.
        /// </summary>
        /// <param name="rule">Match rule.</param>
        /// <param name="isDnAttr">Whether this applies to the dn.</param>
        /// <returns>An attribute value list for which only the equals operator applies.</returns>
        EqualsOnlyAttributeValueList this[Rule rule, bool isDnAttr] { get; }

        /// <summary>
        /// Extended match filter without a dn or attribute specified.
        /// </summary>
        /// <param name="rule">Match rule.</param>
        /// <returns>An attribute value list for which only the equals operator applies.</returns>
        EqualsOnlyAttributeValueList this[Rule rule] { get; }
    }
}
