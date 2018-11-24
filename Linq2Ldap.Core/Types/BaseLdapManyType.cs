using System;
using System.Collections.Generic;
using System.Linq;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types
{
    /// <summary>
    /// Facilitates treatment of attribute value lists as though they were single-valued,
    /// for the purpose of Expressions. Example: (emails=one-of-them*). Here, the emails
    /// attribute would be a list of emails.
    /// </summary>
    /// <typeparam name="T">The underlying attribute type, e.g, string.</typeparam>
    /// <typeparam name="TConv">A converter that can create a List of T from an AttributeValueList.</typeparam>
    public abstract class BaseLdapManyType<T, TConv>: List<T>, ILdapComparable<T>
        where T: IComparable
        where TConv: class, IConverter<List<T>>
    {
        protected AttributeValueList Raw { get; set; }
        public BaseLdapManyType(AttributeValueList raw, TConv conv)
            : base(conv.Convert(raw))
        {
            this.Raw = raw;               
        }

        public static bool operator ==(BaseLdapManyType<T, TConv> a, string b)
            => a?.Any(m => m.CompareTo(b) == 0) ?? b == null;

        public static bool operator !=(BaseLdapManyType<T, TConv> a, string b)
            => !(a == b);

        public static bool operator <(BaseLdapManyType<T, TConv> a, string b)
            => a?.Any(m => m.CompareTo(b) < 0) ?? b == null;

        public static bool operator >(BaseLdapManyType<T, TConv> a, string b)
            => a?.Any(m => m.CompareTo(b) > 0) ?? b == null;

        public static bool operator <=(BaseLdapManyType<T, TConv> a, string b)
            => a?.Any(m => m.CompareTo(b) <= 0) ?? b == null;

        public static bool operator >=(BaseLdapManyType<T, TConv> a, string b)
            => a?.Any(m => m.CompareTo(b) >= 0) ?? b == null;

        public virtual bool StartsWith(string frag) => this.Raw.Any(s => s.ToString().StartsWith(frag));
        public virtual bool EndsWith(string frag) => this.Raw.Any(s => s.ToString().EndsWith(frag));
        public virtual bool Contains(string frag) => this.Raw.Any(s => s.ToString().Contains(frag));

        /// <summary>
        /// Compares a multi-valued LDAP list with the given string.
        /// Warning: by necessity, this is a little quirky (look at return value)
        /// due to the use of implicit operators. Serialization should still work
        /// fine, though.
        /// </summary>
        /// <param name="b">The string to compare with.</param>
        /// <returns>An IntList of individual CompareTo results.</returns>
        public IntList CompareTo(T b) => new IntList(this.Select(s => s.CompareTo(b)));
    }
}