using System.Collections.Generic;

namespace Linq2Ldap.Core.FilterParser {
    public class Tokens2254: Tokens1960 {
        public Tokens2254()
        {
            Lookup = new Dictionary<string, string>() {
                { @"\(",               LeftParen },
                { @"\)",               RightParen },
                { @"\&",               And },
                { @"\|",               Or },
                { @"\!",               Not },
                { @"=\*(?=\s*[\)])", Present },
                { @"=",               Equal },
                { @">=",              GTE },
                { @"<=",              LTE },
                { @"\*",              Star },
                { @"~=",              Approx },
                { @"((?:\\[a-fA-F0-9]{2})*)", HexEscape },
                { @"[^\x01-\x27\x2b-\x5b\x5d-\x7f]+", LegacyToken },
            };
        }

        public const string HexEscape = "HEX";
        public const string LegacyToken = "Legacy";
    }
}