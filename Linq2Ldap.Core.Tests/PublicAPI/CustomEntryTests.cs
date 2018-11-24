using Linq2Ldap.Core.FilterCompiler;
using Linq2Ldap.Core.FilterParser;
using Linq2Ldap.Core.Models;
using Linq2Ldap.Core.Proxies;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.Tests.PublicAPI
{
    public class CustomEntryTests
    {
        public LdapFilterParser Parser;
        public LdapFilterCompiler Compiler;
        public CustomEntryTests()
        {
            Parser = new LdapFilterParser();
            Compiler = new LdapFilterCompiler();
        }

        [Fact]
        public void Parser_CanUseCustomBaseModel()
        {
            var filter = "(mail=something)";
            var expr = Parser.Parse<CustomEntry>(filter);
            var dictOne = new Dictionary<string, AttributeValueList>()
            {
                { "mail", new AttributeValueList("something") }
            };
            var dictTwo = new Dictionary<string, AttributeValueList>()
            {
                { "mail", new AttributeValueList("something else") }
            };

            // No Exceptions thrown during execution implies it correctly used the custom entry type.
            var result = expr.Compile()(new CustomEntry() { Attributes = new EntryAttributeDictionary(dictOne) });
            Assert.True(result);
            result = expr.Compile()(new CustomEntry() { Attributes = new EntryAttributeDictionary(dictTwo) });
            Assert.False(result);
        }

        [Fact]
        public void Compiler_CanUseCustomBaseModel()
        {
            Expression<Func<CustomEntry, bool>> expr = e => e["mail"] == "example@example.com";
            // No Exceptions implies it correctly used the custom entry type.
            var result = Compiler.Compile(expr);
            Assert.Equal("(mail=example@example.com)", result);
        }
    }

    public class CustomEntry : IEntry
    {
        public AttributeValueList this[string key] => Attributes[key];
        public string DistinguishedName { get; set; }
        public EntryAttributeDictionary Attributes { get; set; }
        public bool Has(string attrName) => Attributes.ContainsKey(attrName);
    }
}
