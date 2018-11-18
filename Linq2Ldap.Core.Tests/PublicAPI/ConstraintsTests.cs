using Linq2Ldap.Core.FilterCompiler;
using Linq2Ldap.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.Tests.PublicAPI
{
    public class ConstraintsTests
    {
        private static Assembly assembly;
        static ConstraintsTests()
        {
            assembly = Assembly.GetAssembly(typeof(LdapFilterCompiler));
        }

        [MemberData(nameof(GenericMethodRefsFinder), typeof(Entry))]
        [MemberData(nameof(GenericEntityRefsFinder), typeof(Entry))]
        [Theory]
        // Must always allow user to provide their own implementation of, e.g., IEntry.
        public void NeverUsedAsConstraint(string typeName, string methodName, Type[] args, Type forbidden)
        {
            Assert.All(args, arg =>
            {
                var constraints = arg.GetGenericParameterConstraints();
                Assert.DoesNotContain(constraints, c => c == forbidden);
            });
        }

        public static IEnumerable<object[]> GenericMethodRefsFinder(Type forbidden)
            => assembly.GetTypes()
                .Where(t => t.IsClass || t.IsInterface || t.IsEnum)
                .SelectMany(t => t.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                .Where(t => t.IsGenericMethod)
                .Select(m => new object[] { m.DeclaringType.FullName, m.Name, m.GetGenericArguments(), forbidden });

        public static IEnumerable<object[]> GenericEntityRefsFinder(Type forbidden)
            => assembly.GetTypes()
                .Where(t =>
                    (t.IsClass || t.IsInterface || t.IsEnum)
                    && t.IsGenericTypeDefinition)
                .Select(t => new object[] { t.FullName, null, t.GetGenericArguments(), forbidden } );

    }
}
