using Linq2Ldap.Core.FilterCompiler;
using Xunit;

namespace Linq2Ldap.Core.Tests.FilterCompiler {
    public class ValueUtilTests {

        [InlineData("a*b", @"a\*b")]
        [InlineData("  a*b  ", @"\ \ a\*b\ \ ")]
        [InlineData("   ", @"\ \ \ ")]
        [InlineData(@"\", @"\\")]
        [InlineData(@"abc\", @"abc\\")]
        [Theory]
        public void EscapeFilterValue_(string input, string expected) {
            var util = new ValueUtil();
            var actual = util.EscapeFilterValue(input);
            Assert.Equal(expected, actual);
        }
    }
}