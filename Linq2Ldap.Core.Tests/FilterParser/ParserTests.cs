using Xunit;
using Linq2Ldap.Core.FilterParser;
using Linq2Ldap.Core.Models;
using System.Collections.Generic;
using Linq2Ldap.Core.Proxies;
using Linq2Ldap.Core.FilterCompiler;
using System.Linq.Expressions;
using System;
using Linq2Ldap.Core.FilterCompiler.Models;
using Linq2Ldap.Core.FilterParser.Models;

namespace Linq2Ldap.Core.Tests.FilterParser {
    public class ParserTests {
        public LdapFilterParser Parser;
        public ParserTests() {
            Parser = new LdapFilterParser();
        }

        [Fact]
        public void Parse_Integration_ThrowsOnMissingParens() {
            Assert.Throws<SyntaxException>(() => Parser.Parse<Entry>("a=b"));
        }

        [InlineData("(a=b)", true)]
        [InlineData("(a=c)", false)]
        [InlineData("(!(a=b))", false)]
        [InlineData("(!(a=c))", true)]
        [InlineData("(c=d)", true)]
        [InlineData("(c<=e)", true)]
        [InlineData("(c>=e)", false)]
        [InlineData("(c>=c)", true)]
        [InlineData("(e=31*)", true)]
        [InlineData("(e=*14)", true)]
        [InlineData("(e=*1*)", true)]
        [InlineData("(e=*2*)", false)]
        [InlineData("(e=21*)", false)]
        [InlineData("(e=*15)", false)]
        [InlineData("(&(a=b)(c=d))", true)]
        [InlineData("(&(a=b)(!(c=d)))", false)]
        [InlineData("(|(a=b)(!(c=d)))", true)]
        [InlineData("(&(a=b)(c=d)(e>=31)))", true)]
        [InlineData("(&(a=b)(c=d)(e>=315)))", false)]
        [Theory]
        public void Parse_Integration_BasicBooleans(string input, bool expected) {
            var expr = Parser.Parse<Entry>(input);
            var dict = new Dictionary<string, AttributeValueList>() {
                { "a", new AttributeValueList(new List<object>() { "b" }) },
                { "c", new AttributeValueList(new List<object>() { "d" }) },
                { "e", new AttributeValueList(new List<object>() { "314" }) }
            };
            var entry = new Entry() { Attributes = new EntryAttributeDictionary(dict) };
            Assert.Equal(expected, expr.Compile()(entry));
        }

        [InlineData("(a=*)", true)]
        [InlineData("(multiv=*)", true)]
        [InlineData("(f=*)", false)]
        [InlineData("(emptylist=*)", true)]
        [Theory]
        public void Parse_Integration_ExistenceChecks(string input, bool expected) {
            var expr = Parser.Parse<Entry>(input);
            var dict = new Dictionary<string, AttributeValueList>() {
                { "a", new AttributeValueList(new List<object>() { "b" }) },
                { "multiv", new AttributeValueList(new List<object>() { "d", "e" }) },
                { "emptylist", new AttributeValueList(new List<object>() { }) }
            };
            var entry = new Entry() { Attributes = new EntryAttributeDictionary(dict) };
            Assert.Equal(expected, expr.Compile()(entry));
        }

        [InlineData("(a~=B)", true)]
        [InlineData("(a~=b)", true)]
        [InlineData("(a~=C)", false)]
        [InlineData("(a~=c)", false)]
        [InlineData("(c~=E)", true)]
        [Theory]
        public void Parse_Integration_ApproxChecks(string input, bool expected) {
            var expr = Parser.Parse<Entry>(input);
            var dict = new Dictionary<string, AttributeValueList>() {
                { "a", new AttributeValueList(new List<object>() { "b" }) },
                { "c", new AttributeValueList(new List<object>() { "d", "e" }) },
            };
            var entry = new Entry() { Attributes = new EntryAttributeDictionary(dict) };
            Assert.Equal(expected, expr.Compile()(entry));
        }

        [InlineData(@"(a\==b)", true)]
        [InlineData(@"(a=b)", false)]
        [InlineData(@"(a=*)", false)]
        [InlineData(@"(a\==*)", true)]
        [InlineData(@"(c\\=*)", true)]
        [InlineData(@"(c\==*)", false)]
        [Theory]
        public void Parse_Integration_1960EscapeChecks(string input, bool expected) {
            var expr = Parser.Parse<Entry>(input);
            var dict = new Dictionary<string, AttributeValueList>() {
                { @"a=", new AttributeValueList(new List<object>() { "b" }) },
                { @"c\", new AttributeValueList(new List<object>() { "d", "e" }) },
            };
            var entry = new Entry() { Attributes = new EntryAttributeDictionary(dict) };
            Assert.Equal(expected, expr.Compile()(entry));
        }

        [Fact]
        public void Parse_CanParseCanonicalTrue() {
            var expr = Parser.Parse<Entry>(@"(&)");
            Assert.Equal("m => True", expr.ToString());
            expr = Parser.Parse<Entry>(@"(|(&)(a=b)(c=d))");
            Assert.StartsWith("m => (True OrElse", expr.ToString());
        }

        [Fact]
        public void Parse_CanParseCanonicalFalse() {
            var expr = Parser.Parse<Entry>(@"(|)");
            Assert.Equal("m => False", expr.ToString());
            expr = Parser.Parse<Entry>(@"(&(|)(a=b)(c=d))");
            Assert.StartsWith("m => (False AndAlso", expr.ToString());
        }

        [InlineData(@"(  & (a =  b ) ( c= d) )", @"(&(a=b)(c=d))")]
        [InlineData(@"(  &  )", @"(&)")]
        [InlineData(@"(  |)", @"(|)")]
        [InlineData(@"(| (aaaa=         123  )(bcd\ = \  321 ))", @"(|(aaaa=123)(bcd\ =\ \ 321))")]
        [Theory]
        public void Parse_1960CanHandleWhitespace(string input, string expected) {
            var compiler = new LdapFilterCompiler(null, new CompilerOptions() { Target = RFCTarget.RFC1960 });
            var expr = Parser.Parse<Entry>(input);
            var filter = compiler.Compile(expr);
            Assert.Equal(expected, filter);
        }

        [InlineData(@"(one=two=three=four)", "two=three=four")]
        [InlineData(@"(one=three\*four\*five)", @"three*four*five")]
        [InlineData(@"(one=three\(four\(five)", @"three(four(five")]
        [InlineData(@"(one=three\)four\)five)", @"three)four)five")]
        [InlineData(@"(one=three\(four\)five)", @"three(four)five")]
        [InlineData(@"(one=three\(\*four\)five)", @"three(*four)five")]
        [InlineData(@"(one=three\\four\\five)", @"three\four\five")]
        [Theory]
        public void Parse_1960UnescapesLeftvalue(string testStr, string expected)
        {
            var expr = Parser.Parse<Entry>(testStr);
            var bin = expr.Body as BinaryExpression;
            var right = (bin.Right as ConstantExpression).Value;
            Assert.Equal(expected, right);
        }

        [InlineData(@"(member:1.2.840.113556.1.4.1941:=abc)", "abc")]
        [InlineData(@"(:dn:=abc)", "abc")]
        [InlineData(@"(:dn:=CN=someuser,OU=Users,DC=ACompany,DC=Com)", "CN=someuser,OU=Users,DC=ACompany,DC=Com")]
        [InlineData(@"(member:1.2.840.113556.1.4.1941:=CN=someuser,OU=Users,DC=ACompany,DC=Com)", "CN=someuser,OU=Users,DC=ACompany,DC=Com")]
        [Theory]
        public void Parse_1960CanHandleValuesWithSymbols(string testStr, string expected)
        {
            var expr = Parser.Parse<Entry>(testStr);
            var bin = expr.Body as BinaryExpression;
            var right = (bin.Right as ConstantExpression).Value;
            Assert.Equal(expected, right);
        }
    }
}