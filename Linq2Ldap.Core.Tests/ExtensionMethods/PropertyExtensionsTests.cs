using Xunit;
using System.Collections;
using System.Collections.Generic;
using Linq2Ldap.Core.ExtensionMethods;
using Linq2Ldap.Core.Types;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Tests.ExtensionMethods {
    public class PropertyExtensionMethodsTests {
        [InlineData("", "*", true)]
        [InlineData("asdfaf", "*", true)]
        [InlineData("***", "*", true)]
        [InlineData("asdf", "as*", true)]
        [InlineData("university of iowa", "univ*of*iowa", true)]
        [InlineData("repeat replay rep", "rep*rep*rep", true)]
        [InlineData("repeat replay rep", "rep*rep*rep*", true)]
        [InlineData("repeat replay rep", "*rep*rep*rep", true)]
        [InlineData("repeat replay rep", "*rep*rep*rep*", true)]
        [InlineData("repeat replay rep", "*rep*replay", false)]
        [InlineData("university of iowa", "univ*of*il", false)]
        [InlineData("university of iowa", "univ*iowa", true)]
        [InlineData("university of iowa", "univ*", true)]
        [InlineData("university of iowa", "*iowa", true)]
        [InlineData("university of iowa", "univ*for*iowa", false)]
        [InlineData(null, "univ*for*iowa", false)]
        [InlineData(null, "*", false)]
        [Theory]
        public void Matches_MatchesValidRFC1960RightHandStrings(string input, string pattern, bool expectedResult) {
            var actual = input.Matches(pattern);
            Assert.Equal(expectedResult, actual);
        }

        [InlineData(new [] { "university of iowa" }, "univ*of*iowa", true)]
        [InlineData(new [] { "university of iowa" }, "*", true)]
        [InlineData(new string[] { }, "*", true)]
        [InlineData(null, "*", false)]
        [Theory]
        public void Matches_LdapStringList_MatchesValidRFC1960RightHandStrings(string[] input, string pattern, bool expectedResult) {
            LdapStringList i = input == null ? (LdapStringList)null : input;
            Assert.Equal(expectedResult, i.Matches(pattern));
        }

        [InlineData("university of IOWA", "*iowa", true)]
        [InlineData("university of IOWA", "univ*for*iowa", false)]
        [Theory]
        public void Approx_MatchesInvariant(string input, string pattern, bool expectedResult) {
            var actual = input.Approx(pattern);
            Assert.Equal(expectedResult, actual);
        }

        
        [InlineData(new object[] { 314 }, "*314", true)]
        [InlineData(new object[] { 31415 }, "3*4*5", true)]
        [InlineData(new object[] { 31415 }, "3*5*5", false)]
        [InlineData(new object[] { }, "*", true)]
        [InlineData(null, "*", false)]
        [Theory]
        public void Matches_LdapInt_MatchesInvariant(object[] input, string pattern, bool expectedResult) {
            LdapInt i = input == null
                ? null
                : new LdapInt(new AttributeValueList(
                    new List<object>(input)));
            var actual = i.Matches(pattern);
            Assert.Equal(expectedResult, actual);
        }

        [InlineData(new object[] { 314 }, "*314", true)]
        [InlineData(new object[] { 31415 }, "3*4*5", true)]
        [InlineData(new object[] { 31415 }, "3*5*5", false)]
        [InlineData(new object[] { }, "*", true)]
        [InlineData(null, "*", false)]
        [Theory]
        public void Approx_LdapInt_MatchesInvariant(object[] input, string pattern, bool expectedResult) {
            LdapInt i = null;
            if (input != null)
                i = new LdapInt(new AttributeValueList(new List<object>(input)));

            var actual = i.Approx(pattern);
            Assert.Equal(expectedResult, actual);
        }

        [InlineData(new object[] { "abc", "123", "456" }, "ab*", true)]
        [Theory]
        public void Matches_ProxyValues(object[] input, string pattern, bool expected) {
            var p = new AttributeValueList(new List<object>(input));
            var actual = p.Matches(pattern);
            Assert.Equal(expected, actual);
        }
        
        [InlineData(new object[] { "abc", "123", "456" }, "ab*", true)]
        [Theory]
        public void Approx_ProxyValues(object[] input, string pattern, bool expected) {
            var p = new AttributeValueList(new List<object>(input));
            var actual = p.Approx(pattern);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Any_ProxyValues_MatchesAny()
        {
            var input = new object[] { "abc", "123", "456" };
            var p = new AttributeValueList(new List<object>(input));
            Assert.True(p.Any());
        }

        [Fact]
        public void Any_LdapInt_MatchesAny()
        {
            var input = new object[] { 123, 456, 789 };
            var p = new LdapInt(new AttributeValueList(new List<object>(input)));
            Assert.True(p.Any());
        }

        [Fact]
        public void Any_LdapStringList_MatchesAny()
        {
            var input = new string[] { "abc", "123", "456" };
            var p = (LdapStringList)input;
            Assert.True(p.Any());
        }

        [Fact]
        public void Any_MatchesAny()
        {
            var input = "somestring";
            Assert.True(input.Any());
        }

    }
}