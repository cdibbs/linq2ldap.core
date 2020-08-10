using System;
using System.Linq;
using System.Linq.Expressions;
using Linq2Ldap.Core.ExtensionMethods;
using Linq2Ldap.Core.Tests.FilterCompiler;
using Xunit;

namespace Linq2Ldap.Core.Tests.ExtensionMethods
{
    public class GuidExtensionsTests
    {
        [Fact]
        public void ToEscapedBytesString_Guid()
        {
            var guid = new Guid("96AA667A-E58E-486D-9114-CB1EDC5E2B0D");
            var result = guid.ToEscapedBytesString();
            Assert.Equal(@"\7a\66\aa\96\8e\e5\6d\48\91\14\cb\1e\dc\5e\2b\0d", result);
        }

        [Fact]
        public void ToEscapedBytesString_BytesArray()
        {
            var bytes = new Guid("96AA667A-E58E-486D-9114-CB1EDC5E2B0D").ToByteArray();
            var result = bytes.ToEscapedBytesString();
            Assert.Equal(@"\7a\66\aa\96\8e\e5\6d\48\91\14\cb\1e\dc\5e\2b\0d", result);
        }
    }
}