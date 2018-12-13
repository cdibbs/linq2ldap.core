using Linq2Ldap.Core.Models;
using Linq2Ldap.Core.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.IntegrationTest
{
    public class LinqTests
    {
        [Fact]
        public void CheckActualLinqSyntaxWorks()
        {
            var entries = new List<Entry>();
            for (int i=0; i<10; i++)
            {
                var e = new Entry()
                {
                    DistinguishedName = "something",
                    Attributes = new EntryAttributeDictionary()
                    {
                        { "cn", new AttributeValueList(i % 2 == 0 ? "john" + i : "rick" + i) }
                    }
                };
                entries.Add(e);
            }

            var mine = from entry in entries
                       where entry["cn"].StartsWith("john")
                       select entry["cn"];

            Assert.Equal(entries.Count() / 2, mine.Count());
            Assert.All(mine, e => e.StartsWith("john"));
        }
    }
}
