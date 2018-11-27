using System;
using System.Collections.Generic;
using System.Text;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types
{
    public class StringConverter : IConverter<string>
    {
        public string Convert(AttributeValueList values)
        {
            if (values.Count > 0)
            {
                if (values[0] is string s)
                    return s;
                if (values[0] is Byte[] b)
                    return Encoding.UTF8.GetString(b);

                throw new FormatException(
                    $"Expected string or Byte[] but got type {values[0].GetType().Name}. Please create custom converter.");
            }

            return null;
        }
    }
}
