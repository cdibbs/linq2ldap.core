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
        public static string ToEscapedBytesString(this Guid guid) => 
            guid.ToByteArray().ToEscapedBytesString();

        /// <summary>
        /// Return a string of bytes of the source GUID
        /// </summary>
        /// <param name="array">The bytes to return</param>
        /// <returns></returns>
        public static string ToEscapedBytesString(this byte[] array) => 
            string.Join("", array.Select(b => $"\\{b:x2}"));
    }
}