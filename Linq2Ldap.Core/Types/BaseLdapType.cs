using System;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types
{
    public abstract class BaseLdapType<T, TConv>: ILdapComparable<T>
        where T: IComparable
        where TConv : class, IConverter<T>
    {
        internal protected PropertyValueCollection Raw { get; set; }
        internal protected T Converted { get; set; }
        public BaseLdapType(PropertyValueCollection raw, TConv conv) 
        {
            this.Raw = raw;
            this.Converted = conv.Convert(Raw);
        }

        public static bool operator ==(BaseLdapType<T, TConv> a, T b)
            => a == null ? b == null : a.Raw.Count > 0 && a._CompareTo(b) == 0;

        public static bool operator !=(BaseLdapType<T, TConv> a, T b)
            => a == null ? b != null : a.Raw.Count == 0 || a._CompareTo(b) != 0;

        public static bool operator <(BaseLdapType<T, TConv> a, T b)
            => a == null ? false : a.Raw.Count > 0 && a._CompareTo(b) < 0;

        public static bool operator >(BaseLdapType<T, TConv> a, T b)
            => a == null ? false : a.Raw.Count > 0 && a._CompareTo(b) > 0;

        public static bool operator <=(BaseLdapType<T, TConv> a, T b)
            => a == null ? false : a.Raw.Count > 0 && a._CompareTo(b) <= 0;

        public static bool operator >=(BaseLdapType<T, TConv> a, T b)
            => a == null ? false : a.Raw.Count > 0 && a._CompareTo(b) >= 0;
        public static implicit operator string(BaseLdapType<T, TConv> source)
            => source.ToString();

        public virtual bool StartsWith(string frag) => Raw.Count > 0 && this.ToString().StartsWith(frag);
        public virtual bool EndsWith(string frag) => Raw.Count > 0 && this.ToString().EndsWith(frag);
        public virtual bool Contains(string frag) => Raw.Count > 0 && this.ToString().Contains(frag);

        /* We need this, internally, but it can be misleading to have it public in the case of certain types
         * like ints. Thus, we'll leave it up to the sub-classers whether they want to implement IComparable
         * and add a public wrapper for their protected _CompareTo.
         */
        protected abstract int _CompareTo(object b);

        public IntList CompareTo(T b) => _CompareTo(b);

        public override string ToString() {
            return Raw.Count > 0 ? Raw[0].ToString() : null;
        }
    }
}