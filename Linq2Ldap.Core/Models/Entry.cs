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

        public AttributeValueList this[string attr]
            => this.Attributes.ContainsKey(attr) ? this.Attributes[attr] : null;

        public EqualsOnlyAttributeValueList this[string attr, Rule rule, bool isDn]
            => this.Attributes.ContainsKey(attr) ? this.Attributes[attr] : null;

        public EqualsOnlyAttributeValueList this[string attr, Rule rule]
            => this[attr, rule, false];

        public EqualsOnlyAttributeValueList this[string attr, bool isDn]
            => this[attr, null, isDn];

        public EqualsOnlyAttributeValueList this[Rule rule, bool isDn]
            => this[null, rule, isDn];

        public EqualsOnlyAttributeValueList this[Rule rule]
            => this[null, rule, false];
    }
}
