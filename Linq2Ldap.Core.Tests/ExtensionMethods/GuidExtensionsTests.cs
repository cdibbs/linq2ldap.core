using System;
using Linq2Ldap.Core.ExtensionMethods;
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
            var bytes = new byte[] {0xAB, 0x12, 0xFF, 0x00};
            var result = bytes.ToEscapedBytesString();
            Assert.Equal(@"\ab\12\ff\00", result);
        }
    }
}