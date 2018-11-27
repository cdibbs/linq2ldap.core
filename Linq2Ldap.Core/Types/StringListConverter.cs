using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types {
    public class StringListConverter : IConverter<List<string>>
    {
        public List<string> Convert(AttributeValueList values)
        {
            return values == null
                ? null
                : values.Select(ConvertOne).ToList();
        }

        protected string ConvertOne(object o)
        {
            if (o is string s)
                return s;
            if (o is Byte[] b)
                return Encoding.UTF8.GetString(b);

            throw new FormatException(
                $"Expected string or Byte[] but got type {o.GetType().Name}. Please create custom converter.");
        }
    }
}