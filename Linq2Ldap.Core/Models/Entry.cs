using System.Collections;
using System.Collections.Generic;
using Linq2Ldap.Core.Attributes;
using Linq2Ldap.Core.Proxies;
using Linq2Ldap.Core.Types;

namespace Linq2Ldap.Core.Models
{
    /// <summary>
    /// Represents a DirectoryEntry in a platform-agnostic way.
    /// </summary>
    public class Entry: IEntry
    {
        public string DistinguishedName { get; set; }

        public EntryAttributeDictionary Attributes { get; set; }

        public virtual bool Has(string attrName) => this.Attributes.ContainsKey(attrName);

        public AttributeValueList this[string key] {
            get => this.Attributes.ContainsKey(key) ? this.Attributes[key] : null;
        }
    }
}
