using System;
using System.Collections.Generic;
using System.Text;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types
{
    public class StringConverter : IConverter<string>
    {
        public string Convert(PropertyValueCollection values)
        {
            if (values.Count > 0)
            {
                return values[0] as string ?? values[0].ToString();
            }

            return null;
        }
    }
}
