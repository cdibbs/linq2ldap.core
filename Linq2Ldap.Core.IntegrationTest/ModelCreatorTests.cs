using Linq2Ldap.Core.Proxies;
using Linq2Ldap.Core.Types;
using System;
using System.Collections.Generic;
using Xunit;

namespace Linq2Ldap.Core.IntegrationTest
{
    public class ModelCreatorTests
    {
        public IModelCreator ModelCreator { get; set; }
        public ModelCreatorTests()
        {
            ModelCreator = new ModelCreator();
        }

        [Fact]
        public void Create_CanConvertCustomFieldTypes()
        {
            var props = new Dictionary<string, PropertyValueCollection>()
            {
                { "mail", new PropertyValueCollection("something@example.com") },
                { "alt-mails", new PropertyValueCollection("one@two.com", "three@four.com") },
                { "number", new PropertyValueCollection(123) }
            };
            var properties = new DirectoryEntryPropertyCollection(props);
            var result = ModelCreator.Create<MyModel>(properties, "bogus path");

            Assert.Equal(props["mail"][0] as string, result.Mail);
            Assert.Equal(result.Mail2, result.Mail);
            Assert.Equal(result.Number2, (int)result.Number);
        }
    }
}
