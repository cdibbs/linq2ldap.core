using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Linq2Ldap.Core.FilterCompiler;
using Linq2Ldap.Core.FilterCompiler.Models;
using Linq2Ldap.Core.Models;
using Xunit;

namespace Linq2Ldap.Core.Tests.FilterCompiler
{
    public class LdapFilterCompilerTests
    {
        private LdapFilterCompiler FilterCompiler;
        private CompilerCore Core;
        public LdapFilterCompilerTests()
        {
            FilterCompiler = new LdapFilterCompiler();
            Core = new CompilerCore(new CompilerOptions());
        }

        [Fact]
        public void _MemberToString_NonDataSourceModel_Throws()
        {
            var scopedModel = new { Weird = 123 };
            Expression<Func<TestLdapModel, bool>> expr1 = (TestLdapModel u) => scopedModel.Weird == 123;
            var member = ((BinaryExpression)expr1.Body).Left;
            Action lambda = () => Core._MemberToString(member as MemberExpression, expr1.Parameters);
            Assert.Throws<NotImplementedException>(lambda);
        }

        [Fact]
        public void Compile_EscapesIndexerNames() {
            Expression<Func<TestLdapModel, bool>> expr = (TestLdapModel e) => e["one "] == "123";
            var filter = Core.ExpressionToString(expr.Body, expr.Parameters);
            Assert.Equal(@"(one\20=123)", filter);
        }

        [Fact]
        public void Compile_EscapesPropertyNames() {
            Expression<Func<TestLdapModel, bool>> expr = (TestLdapModel e) => e.WeirdName == "123";
            var filter = Core.ExpressionToString(expr.Body, expr.Parameters);
            Assert.Equal(@"(\20we ird\20\20=123)", filter);
        }

        [Fact]
        public void _MemberToString_DataSourceModel_SerializesByColumnAttrWhenAvailable()
        {
            Expression<Func<TestLdapModel, bool>> expr1 = (TestLdapModel u) => u.SamAccountName == "something";
            var member = ((BinaryExpression)expr1.Body).Left;
            var result = Core._MemberToString(member as MemberExpression, expr1.Parameters);
            Assert.Equal("samaccountname", result);

            Expression<Func<TestUser, bool>> expr2 = (TestUser u) => u.UserlandProp == "something";
            member = ((BinaryExpression)expr2.Body).Left;
            result = Core._MemberToString(member as MemberExpression, expr2.Parameters);
            Assert.Equal("UserlandProp", result);
        }

        [Fact]
        public void Compile_NonConstPDictKey_Throws() // KIS: why maintain unnec. complexity?
        {
            Func<string> testfn = () => "samaccountname"; // TestLdapModel can always invoke prior to express
            Expression<Func<TestLdapModel, bool>> expr1 = (TestLdapModel u) => u.Attributes[testfn()] == "123";
            Assert.Throws<NotImplementedException>(() => FilterCompiler.Compile(expr1));
        }

        [Fact]
        public void Compile_ExtendedIndexes_DnFilter()
        {
            Expression<Func<Entry, bool>> expr = e => e["ou", true] == "something";
            var result = FilterCompiler.Compile(expr);
            Assert.Equal("(ou:dn:=something)", result);
        }

        [Fact]
        public void Compile_ExtendedIndexes_DnFilterWithRule()
        {
            Expression<Func<Entry, bool>> expr = e => e["ou", Rule.InChain, true] == "something";
            var result = FilterCompiler.Compile(expr);
            Assert.Equal($"(ou:dn:{Rule.InChain.RuleCode}:=something)", result);
        }

        [Fact]
        public void Compile_ExtendedIndexes_FilterWithRule()
        {
            Expression<Func<Entry, bool>> expr = e => e["member", Rule.InChain] == "something";
            var result = FilterCompiler.Compile(expr);
            Assert.Equal($"(member:{Rule.InChain.RuleCode}:=something)", result);
        }

        [Fact]
        public void Compile_AndAlsoWithSubExpr_GeneratesValidLDAPFilterString()
        {
            Expression<Func<TestLdapModel, bool>> e
                = (TestLdapModel u) => u.SamAccountName.Contains("test") && u.CommonName == "123";
            var result = FilterCompiler.Compile(e);
            Assert.Equal("(&(samaccountname=*test*)(cn=123))", result);
        }

        [Fact]
        public void Compile_StringCompare_NonConstParam()
        {
            Func<string> testfn = () => "samaccountname"; // TestLdapModel can always invoke prior to express
            Expression<Func<TestLdapModel, bool>> e
                = (TestLdapModel u) => u.SamAccountName.Contains(testfn());
            Assert.Throws<NotImplementedException>(() => FilterCompiler.Compile(e));
        }

        [Fact]
        public void Compile_UnsupportedExpressionType_Throws()
        {
            int a = 3, b = 4;
            Expression<Func<TestLdapModel, bool>> e
                = (TestLdapModel u) => a + b == 7;
            Assert.Throws<NotImplementedException>(() => FilterCompiler.Compile(e));
        }

        [Fact]
        public void _EvalExpr_ThrowsWhenNotRecognizedType()
        {
            Expression<Func<TestLdapModel, bool>> e = (TestLdapModel u) => u.SamAccountName.Contains("asdf");
            Assert.Throws<NotImplementedException>(() => Core.EvalExpr(e.Body, e.Parameters));
        }

        [Fact]
        public void Compile_EscapesValues() {
            Expression<Func<TestLdapModel, bool>> e = (TestLdapModel u) => u.CommonName == @"must*escape\this(please)";
            var result = FilterCompiler.Compile(e);
            Assert.Equal(@"(cn=must\2aescape\5cthis\28please\29)", result);
        }
    }

    public class TestUser : TestLdapModel
    {
        public string UserlandProp { get; set; }
    }
}
