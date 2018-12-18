using Linq2Ldap.Core.FilterCompiler.Models;
using Linq2Ldap.Core.FilterParser.Models;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Linq2Ldap.Core.FilterCompiler {
    public class ValueUtil {
        protected CompilerOptions Options { get; set; }
        public ValueUtil(CompilerOptions options)
        {
            Options = options;
        }

        public string EscapeFilterValue(string value) {
            switch (Options.Target)
            {
                case RFCTarget.RFC1960: return LegacyEscape(value);
                case RFCTarget.RFC2254:
                case RFCTarget.RFC4515: return Rfc4515Escape(value);
                default:
                    throw new NotImplementedException($"Escaping not implemented for filter type {Options.Target:G}.");
            }
        }

        internal protected string Rfc4515Escape(string value)
        {
            var result = value.Replace(@"\", @"\5c");
            result = result.Replace("\0", @"\00");
            result = result.Replace(@"(", @"\28");
            result = result.Replace(@")", @"\29");
            result = result.Replace(@"*", @"\2a");
            char maxUnescaped = Convert.ToChar(0x7f);
            result = string.Join("", result
                .Select(c => c > maxUnescaped ? $"\\{c:00X}" : Encoding.UTF8.GetString(new[] { (byte)c }))
                .ToList());
            var left = result.TrimStart(' ');
            var right = result.TrimEnd(' ');
            var middle = result.Trim(' ');
            left = string.Concat(Enumerable.Repeat(@"\20", result.Length - left.Length));
            if (middle.Length == 0)
            {
                return left;
            }
            right = string.Concat(Enumerable.Repeat(@"\20", result.Length - right.Length));
            return left + middle + right;
        }

        internal protected string LegacyEscape(string value)
        {
            var replacer = new Regex(@"([*\)\(\\])");
            var result = replacer.Replace(value, @"\$1");
            var left = result.TrimStart(' ');
            var right = result.TrimEnd(' ');
            var middle = result.Trim(' ');
            left = string.Concat(Enumerable.Repeat(@"\ ", result.Length - left.Length));
            if (middle.Length == 0)
            {
                return left;
            }
            right = string.Concat(Enumerable.Repeat(@"\ ", result.Length - right.Length));
            return left + middle + right;
        }
    }
}