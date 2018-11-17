using System;
using System.Collections.Generic;
using System.Text;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.Types
{
    public class IntConverter : IConverter<int>
    {
        public int Convert(PropertyValueCollection values)
        {
            if (values == null)
            {
                throw new InvalidOperationException("LdapInt value access from null property bag.");
            }

            if (values.Count == 0)
            {
                return 0;
            }

            if (values[0].GetType() == typeof(int))
            {
                return (int)values[0];
            }
            return int.Parse(values[0].ToString());
        }
    }
}
