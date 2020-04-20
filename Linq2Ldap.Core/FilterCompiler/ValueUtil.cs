using Linq2Ldap.Core.FilterCompiler.Models;
using Linq2Ldap.Core.FilterParser.Models;
using System;
using System.Collections.Generic;
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
            var result = new string(value
                .SelectMany(EscapeUtf1Subset)
                .ToArray());

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

        private static IEnumerable<char> EscapeUtf1Subset(char c)
        {
            if (c >= 0x01 && c <= 0x27 ||
                c >= 0x2B && c <= 0x5B ||
                c >= 0x5D && c <= 0x7F)
            {
                // https://tools.ietf.org/search/rfc4515#page-4 - see 'UTF1SUBSET'
                yield return c;
            }
            else
            {
                // https://tools.ietf.org/search/rfc4515#page-4 - see 'escaped'
                foreach (var @byte in Encoding.UTF8.GetBytes(new[] { c }))
                {
                    yield return '\\';
                    yield return ((@byte & 0xF0) >> 4).ToString("x")[0];
                    yield return (@byte & 0x0F).ToString("x")[0];
                }
            }
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