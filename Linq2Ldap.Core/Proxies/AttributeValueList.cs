using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq2Ldap.Core.Types;

namespace Linq2Ldap.Core.Proxies
{
    /// <summary>
    /// A list of values associated with a particular attribute on a directory entry.
    /// </summary>
    public class AttributeValueList
        : EqualsOnlyAttributeValueList
    {
        public AttributeValueList(): base() {}
        public AttributeValueList(params object[] objects) : base(objects) { }
        public AttributeValueList(params string[] objects) : base(objects) { }
        public AttributeValueList(IEnumerable proxyValues): base(proxyValues.Cast<object>())
        {
        }

        public IntList CompareTo(string other)
        {
            var results = new IntList();
            foreach (var v in this) {
                results.Add(string.CompareOrdinal(v.ToString(), other));
            }
            return results;
        }

        public static bool operator ==(AttributeValueList a, string b)
            => a?.Any(m => string.CompareOrdinal(m.ToString(), b) == 0)
                ?? b == null;

        public static bool operator !=(AttributeValueList a, string b)
            => !(a == b);

        public static bool operator <(AttributeValueList a, string b)
            => a?.Any(m => string.CompareOrdinal(m.ToString(), b) < 0)
                ?? throw new ArgumentException("Arguments to < cannot be null.");

        public static bool operator >(AttributeValueList a, string b)
            => a?.Any(m => string.CompareOrdinal(m.ToString(), b) > 0)
                ?? throw new ArgumentException("Arguments to > cannot be null.");

        public static bool operator <=(AttributeValueList a, string b)
            => a?.Any(m => string.CompareOrdinal(m.ToString(), b) <= 0)
                ?? throw new ArgumentException("Arguments to <= cannot be null.");

        public static bool operator >=(AttributeValueList a, string b)
            => a?.Any(m => string.CompareOrdinal(m.ToString(), b) >= 0)
                ?? throw new ArgumentException("Arguments to >= cannot be null.");

        public static implicit operator AttributeValueList(string[] list)
            => new AttributeValueList(new List<object>(list));

        public static implicit operator AttributeValueList(ReadOnlyCollectionBase list)
            => new AttributeValueList(list);

        public static implicit operator AttributeValueList(CollectionBase list)
            => new AttributeValueList(list);

        public bool StartsWith(string frag) => this.Any(s => s.ToString().StartsWith(frag));
        public bool EndsWith(string frag) => this.Any(s => s.ToString().EndsWith(frag));
        public bool Contains(string frag) => this.Any(s => s.ToString().Contains(frag));
    }
}
