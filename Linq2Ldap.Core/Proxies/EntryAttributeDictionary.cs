using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.Proxies
{
    /// <summary>
    /// A dictionary of attributes on a directory entry. Each key is the attribute name.
    /// For example, objectClass, cn, or email.
    /// </summary>
    public class EntryAttributeDictionary: Dictionary<string, AttributeValueList>
    {
        public EntryAttributeDictionary(): base() {}
        public EntryAttributeDictionary(IDictionary results)
        {
            foreach (var key in results.Keys) {
                if (results[key] is ICollection c) {
                    Add(key as string, new AttributeValueList(c));
                } else if (results[key] is AttributeValueList p) {
                    Add(key as string, p);
                } else {
                    throw new ArgumentException(
                        $"Dictionary value must be ReadOnlyCollectionBase or {nameof(AttributeValueList)}. Was: {results[key]?.GetType()}.");
                }
            }
        }

        public static implicit operator EntryAttributeDictionary(DictionaryBase col)
            => new EntryAttributeDictionary(col);
    }
}
