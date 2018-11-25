using Linq2Ldap.Core.FilterCompiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.Tests.PublicAPI
{
    public class TestabilityTests
    {
        private static Assembly assembly;
        static TestabilityTests()
        {
            assembly = Assembly.GetAssembly(typeof(LdapFilterCompiler));
        }

        [MemberData(nameof(PublicDeclaredInstanceMethods))]
        [Theory]
        // The intention is to ensure mockability
        public void PublicDeclaredInstanceMethods_HaveInterfaceMethodOrAreVirtualMockable(
            string typeName, string methodName, MethodInfo m)
        {
            var iface = m.DeclaringType
                .GetInterfaces()
                .SingleOrDefault(i => i.Name == $"I{m.DeclaringType.Name}");
            if (iface != null)
            {
                var map = m.DeclaringType.GetInterfaceMap(iface);
                var index = Array.IndexOf(map.TargetMethods, m);
                if (index == -1)
                {
                    // Must be able to construct type and mock it.
                    // TODO: improve by verifying constructor parameters public constructable, recursively.
                    Assert.Contains(m.DeclaringType.GetConstructors(), c => c.IsPublic);
                    Assert.False(m.DeclaringType.IsSealed);
                    Assert.True(m.IsVirtual);
                }
            }
        }

        public static IEnumerable<object[]> PublicDeclaredInstanceMethods()
            => assembly.GetTypes()
                .Where(t => t.IsClass)
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                .Select((m, i) => new object[] { m.DeclaringType.FullName, m.Name, m });
    }
}
