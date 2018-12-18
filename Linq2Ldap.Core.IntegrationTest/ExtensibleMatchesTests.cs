using Linq2Ldap.Core.FilterCompiler;
using Linq2Ldap.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.IntegrationTest
{
    public class ExtensibleMatchesTests
    {
        LdapFilterCompiler Compiler;
        public ExtensibleMatchesTests()
        {
            Compiler = new LdapFilterCompiler();
        }

        [Fact]
        public void CanCreateDnMatchRules()
        {
            Expression<Func<Entry, bool>> expr
                = e => e[Rule.DnWithData, true] == "something";
            var result = Compiler.Compile(expr);
            Assert.Equal($"(:dn:{Rule.DnWithData.RuleCode}:=something)", result);
        }

        [Fact]
        public void CanCreateDnOnlyRule()
        {
            Expression<Func<Entry, bool>> expr
                = e => e["o", true] == "something";
            var result = Compiler.Compile(expr);
            Assert.Equal($"(o:dn:=something)", result);
        }

        [Fact]
        public void CanCreateMembershipMatch()
        {
            var dn = "CN=someone,OU=people,DC=org";
            Expression<Func<Entry, bool>> expr
                = e => e["memberof", Rule.InChain] == dn;
            var result = Compiler.Compile(expr);
            Assert.Equal($"(memberof:{Rule.InChain.RuleCode}:={dn})", result);
        }
    }
}
