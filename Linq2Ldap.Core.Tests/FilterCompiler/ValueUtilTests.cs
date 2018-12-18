using Linq2Ldap.Core.FilterCompiler;
using Linq2Ldap.Core.FilterCompiler.Models;
using Linq2Ldap.Core.FilterParser.Models;
using Xunit;

namespace Linq2Ldap.Core.Tests.FilterCompiler {
    public class ValueUtilTests {

        [InlineData("a*b", @"a\*b")]
        [InlineData("  a*b  ", @"\ \ a\*b\ \ ")]
        [InlineData("   ", @"\ \ \ ")]
        [InlineData(@"\", @"\\")]
        [InlineData(@"abc\", @"abc\\")]
        [Theory]
        public void EscapeFilterValue_RFC1960(string input, string expected) {
            var util = new ValueUtil(new CompilerOptions() { Target = RFCTarget.RFC1960 });
            var actual = util.EscapeFilterValue(input);
            Assert.Equal(expected, actual);
        }

        [InlineData("a*b", @"a\2ab")]
        [InlineData("  a*b  ", @"\20\20a\2ab\20\20")]
        [InlineData("   ", @"\20\20\20")]
        [InlineData(@"\", @"\5c")]
        [InlineData(@")", @"\29")]
        [InlineData(@"(", @"\28")]
        [InlineData(@"abc\", @"abc\5c")]
        [Theory]
        public void EscapeFilterValue_RFC2254(string input, string expected)
        {
            var util = new ValueUtil(new CompilerOptions() { Target = RFCTarget.RFC2254 });
            var actual = util.EscapeFilterValue(input);
            Assert.Equal(expected, actual);
        }

        [InlineData("a*b", @"a\2ab")]
        [InlineData("  a*b  ", @"\20\20a\2ab\20\20")]
        [InlineData("   ", @"\20\20\20")]
        [InlineData(@"\", @"\5c")]
        [InlineData(@")", @"\29")]
        [InlineData(@"(", @"\28")]
        [InlineData(@"abc\", @"abc\5c")]
        [Theory]
        public void EscapeFilterValue_RFC4515(string input, string expected)
        {
            var util = new ValueUtil(new CompilerOptions() { Target = RFCTarget.RFC4515 });
            var actual = util.EscapeFilterValue(input);
            Assert.Equal(expected, actual);
        }
    }
}