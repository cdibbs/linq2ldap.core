using System;
using System.Linq;

namespace Linq2Ldap.Core.ExtensionMethods
{
    /// <summary>
    /// Helper class for working with GUID
    /// </summary>
    public static class GuidExtensions
    {
        /// <summary>
        /// Return a string of bytes of the source GUID
        /// </summary>
        /// <param name="guid">The GUID to convert</param>
        /// <returns></returns>
        public static object ToEscapedBytesString(this Guid guid)
        {
            return string.Join("", guid.ToByteArray().Select(b => $"\\{b:x2}"));
        }
    }
}