using Linq2Ldap.Core.FilterParser.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.FilterCompiler.Models
{
    public class CompilerOptions
    {
        public RFCTarget Target { get; set; }
            = RFCTarget.RFC4515;
    }
}
