using Linq2Ldap.Core.FilterParser.Models;
using System.Collections.Generic;

namespace Linq2Ldap.Core.FilterParser {
    public interface ILexer
    {
        IEnumerable<Token> Lex(string input);
    }
}