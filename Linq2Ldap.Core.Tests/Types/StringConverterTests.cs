using Linq2Ldap.Core.Proxies;
using Linq2Ldap.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.Tests.Types
{
    public class StringConverterTests
    {
        [Fact]
        public void Convert_CanConvertUtf8Bytes()
        {
            var teststr = "some thing";
            var strBytes = Encoding.UTF8.GetBytes(teststr);
            var conv = new StringConverter();
            var converted = conv.Convert(new AttributeValueList(new[] { strBytes }));
            Assert.Equal(teststr, converted);
        }

        [Fact]
        public void Convert_CanConvertSimpleString()
        {
            var teststr = "some thing";
            var conv = new StringConverter();
            var converted = conv.Convert(new AttributeValueList(teststr));
            Assert.Equal(teststr, converted);
        }

        [Fact]
        public void Convert_ThrowsWhenNotRecognizedType()
        {
            var teststr = new DateTime();
            var conv = new StringConverter();
            Assert.Throws<FormatException>(() => conv.Convert(new AttributeValueList(teststr)));
        }
    }
}
