using System;
using System.Collections.Generic;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types
{
    public class LdapString: BaseLdapType<string, StringConverter>
    {
        public LdapString(AttributeValueList raw): base(raw, new StringConverter())
        {
        }

        public LdapString(AttributeValueList raw, StringConverter conv): base(raw, conv)
        {
        }

        public static implicit operator string(LdapString i)
            => i.Converted;

        public static implicit operator LdapString(string i)
            => new LdapString(new AttributeValueList(i));

        // We'll choose not to make a public version of this for ints, because what would that mean for empty bags?
        protected override int _CompareTo(object b) => Converted.CompareTo(b);

        public override string ToString() => Converted;
    }
}