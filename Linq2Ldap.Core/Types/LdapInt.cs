using System;
using Linq2Ldap.Core.Proxies;
using Linq2Ldap.Core.Types;
using Linq2Ldap.Core.ExtensionMethods;
using System.Collections.Generic;

namespace Linq2Ldap.Core.Types {
    public class LdapInt : BaseLdapType<int, IntConverter>
    {
        public LdapInt(AttributeValueList raw): base(raw, new IntConverter())
        {
        }

        public LdapInt(AttributeValueList raw, IntConverter conv): base(raw, conv)
        {
        }

        public static implicit operator int(LdapInt i) => i.Converted;
        public static implicit operator LdapInt(int i) => new LdapInt(new AttributeValueList(new List<object> { i }));

        // We'll choose not to make a public version of this for ints, because what would that mean for empty bags?
        protected override int _CompareTo(object b) => Converted.CompareTo(b);
    }
}