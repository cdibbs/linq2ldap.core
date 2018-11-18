using Linq2Ldap.Core.Attributes;
using Linq2Ldap.Core.Models;
using Linq2Ldap.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.IntegrationTest
{
    public class MyModel: Entry
    {
        [LdapField("mail")]
        public LdapString Mail { get; set; }

        [LdapField("mail")]
        public string Mail2 { get; set; }

        [LdapField("alt-mails", optional: true)]
        public LdapStringList AltMails { get; set; }

        [LdapField("number")]
        public LdapInt Number { get; set; }

        [LdapField("number")]
        public int Number2 { get; set; }

    }
}
