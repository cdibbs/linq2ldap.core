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
        //ICollection Keys { get; }
        //ICollection Values { get; }
    }
}
