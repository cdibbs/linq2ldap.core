using Linq2Ldap.Core.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Linq2Ldap.Core.Proxies
{
    /// <summary>
    /// Attribute value list type for extended filter matches (which only supports == a.k.a :=).
    /// </summary>
    public class EqualsOnlyAttributeValueList
        : List<object>, IEquatable<string>, ILdapComparable<string>
    {
        public EqualsOnlyAttributeValueList() : base() { }
        public EqualsOnlyAttributeValueList(params object[] objects) : base(objects) { }
        public EqualsOnlyAttributeValueList(params string[] objects) : base(objects) { }
        public EqualsOnlyAttributeValueList(IEnumerable proxyValues) : base(proxyValues.Cast<object>())
        {
        }

        public IntList CompareTo(string other)
        {
            var results = new IntList();
            foreach (var v in this)
            {
                results.Add(string.CompareOrdinal(v.ToString(), other));
            }
            return results;
        }

        public bool Equals(string other) => this == other;

        public override int GetHashCode() => base.GetHashCode();

        public static bool operator ==(EqualsOnlyAttributeValueList a, string b)
            => a?.Any(m => string.CompareOrdinal(m.ToString(), b) == 0)
                ?? b == null;

        public static bool operator !=(EqualsOnlyAttributeValueList a, string b)
            => throw new NotImplementedException("As with extended-match LDAP filters, only == (LDAP :=) is supported.");

        public override bool Equals(object obj) => base.Equals(obj);
    }
}
