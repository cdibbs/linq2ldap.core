using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.Attributes
{
    /// <summary>
    /// Denotes a property on an IEntry implementation to include when loading
    /// LDAP data.
    /// </summary>
    public class LdapFieldAttribute: Attribute
    {
        public string Name { get; set; }
        public bool Optional { get; set; }

        /// <summary>
        /// Marks a property on an IEntry to include when loading LDAP data.
        /// </summary>
        /// <param name="name">The name within the LDAP attributes dictionary.</param>
        /// <param name="optional">Whether this attribute is optional (default: false, throws when missing).</param>
        public LdapFieldAttribute(string name, bool optional = false)
        {
            Name = name;
            Optional = optional;
        }
    }
}
