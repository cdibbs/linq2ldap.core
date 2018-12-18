using System;
using System.Collections.Generic;
using System.Text;

namespace Linq2Ldap.Core.Models
{
    public class Rule
    {
        public string RuleCode { get; }
        public string RuleAlias { get; }

        public Rule(string ruleCode, string ruleAlias)
        {
            RuleCode = ruleCode;
            RuleAlias = ruleAlias;
        }

        /// <summary>
        /// Matches only if all bits from the attribute match the value.
        /// </summary>
        public static readonly Rule BitAnd = new Rule("1.2.840.113556.1.4.803", null);

        /// <summary>
        /// Matches if any bits from the attribute match the value.
        /// </summary>
        public static readonly Rule BitOr = new Rule("1.2.840.113556.1.4.804", null);

        /// <summary>
        /// Same as InChain. Limited to DN filters. Walks ancestry chain
        /// all the way to the root until it finds a match. E.g., group membership
        /// checks like
        /// (memberof:1.2.840.113556.1.4.1941:=cn=Group1,OU=groupsOU,DC=x)
        /// </summary>
        public static readonly Rule TransitiveEval = new Rule("1.2.840.113556.1.4.1941", null);

        /// <summary>
        /// Same as TransitiveEval. Limited to DN filters. Walks ancestry chain
        /// all the way to the root until it finds a match. E.g., group membership
        /// checks like
        /// (memberof:1.2.840.113556.1.4.1941:=cn=Group1,OU=groupsOU,DC=x)
        /// </summary>
        public static readonly Rule InChain = TransitiveEval;

        /// <summary>
        /// Provides a way to match portions of DN values.
        /// </summary>
        public static readonly Rule DnWithData = new Rule("1.2.840.113556.1.4.2253", null);
    }
}
