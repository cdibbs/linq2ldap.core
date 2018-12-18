using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.FilterParser.Models
{
    public class LexerOptions
    {
        /// <summary>
        /// RFC standards target. Default: RFC4515.
        /// </summary>
        public RFCTarget Target { get; set; } = RFCTarget.RFC4515;

        /// <summary>
        /// When true, parser will throw when encountering unescaped, non-RFCTarget characters
        /// in the LDAP filter to parse.
        /// Ex: Non-RFC4515 characters include 0x00, 0x28-0x2a, 0x5c, and > 0x7f.
        /// </summary>
        public bool StrictCharset { get; set; } = true;
    }
}
