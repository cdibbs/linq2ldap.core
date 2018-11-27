using Linq2Ldap.Core.Proxies;
using Linq2Ldap.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.Tests.Types
{
    public class StringListConverterTests
    {
        [Fact]
        public void Convert_CanConvertUtf8Bytes()
        {
            var teststr = "some thing";
            var strBytes = Encoding.UTF8.GetBytes(teststr);
            var conv = new StringListConverter();
            var converted = conv.Convert(new AttributeValueList(new[] { strBytes }));
            Assert.Equal(teststr, converted[0]);
        }

        [Fact]
        public void Convert_CanConvertSimpleString()
        {
            var teststr = "some thing";
            var conv = new StringListConverter();
            var converted = conv.Convert(new AttributeValueList(teststr));
            Assert.Equal(teststr, converted[0]);
        }

        [Fact]
        public void Convert_ThrowsWhenNotRecognizedType()
        {
            var teststr = new DateTime();
            var conv = new StringListConverter();
            Assert.Throws<FormatException>(() => conv.Convert(new AttributeValueList(teststr)));
        }
    }
}
