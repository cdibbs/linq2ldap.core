using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.FilterParser.Models
{
    public enum RFCTarget
    {
        /// <summary>
        /// Earlier RFC that uses \char escaping instead of \hex.
        /// </summary>
        RFC1960 = 1960,

        /// <summary>
        /// Dec 1997 RFC that introduces extended match filters (LDAP v3) and switches to \hex escapes.
        /// </summary>
        RFC2254 = 2254,

        /// <summary>
        /// June 2006 RFC that makes the string representations of LDAP filters suitable
        /// for use in LDAP URLs and elsewhere (UTF8).
        /// </summary>
        RFC4515 = 4515
    }
}
