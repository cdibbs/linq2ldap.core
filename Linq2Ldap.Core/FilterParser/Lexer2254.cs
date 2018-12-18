using Linq2Ldap.Core.FilterParser.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Linq2Ldap.Core.FilterParser
{
    public class Lexer2254: Lexer
    {
        public Lexer2254(LexerOptions options = null)
        {
            Options = options ?? new LexerOptions();
            Tokens = new Tokens2254();
        }

        public override IEnumerable<Token> Lex(string input)
        {
            int i = 0, prevTokEnd = 0;
            Token nextTok = null, ucToken;
            while (i < input.Length)
            {
                switch (nextTok = GetNextToken(input, i, nextTok))
                {
                    case null:
                        i = i + 1;
                        break;
                    case Token t when t.MatchedToken == Tokens2254.HexEscape:
                        i = i + t.Text.Length;
                        break;
                    case Token t when (
                            t.MatchedToken == Tokens2254.LegacyToken
                            && Options.Target == RFCTarget.RFC4515
                    ):
                        i = i + t.Text.Length;
                        if (Options.StrictCharset)
                        {
                            throw new FormatException(
                                string.Join("\n",
                                    $"Unescaped, out-of-range characters found beginning at position: {t.StartPos}."
                                    ,"Are you using the right RFC Target?"
                                ));
                        }

                        break;
                    default:
                        if (null != (ucToken = GetUserCharsToken(input, i, prevTokEnd)))
                        {
                            yield return ucToken;
                        }

                        i = i + nextTok.Text.Length;
                        prevTokEnd = i;
                        yield return nextTok;
                        break;
                }
            }

            if (null != (ucToken = GetUserCharsToken(input, i, prevTokEnd)))
            {
                yield return ucToken;
            }
        }

        protected override Token GetUserCharsToken(string input, int i, int prevTokEnd)
        {
            var raw = input.Substring(prevTokEnd, i - prevTokEnd);
            raw = UnescapeAndTrim(raw);
            if (raw.Length > 0)
            {
                return new Token(raw, i);
            }

            return null;
        }

        protected override string UnescapeAndTrim(string raw)
        {
            raw = string.Join("", Regex.Split(raw, @"^\s+"));
            raw = string.Join("", Regex.Split(raw, @"\s+$"));
            return Regex.Replace(raw, @"\\[a-fA-F0-9]{2}", ReplaceEscapedUnicodeWithChar);
        }

        protected string ReplaceEscapedUnicodeWithChar(Match m)
        {
            var i = int.Parse(m.Value.Substring(1), NumberStyles.HexNumber);
            return new string(new[] { Convert.ToChar(i) });
        }
    }
}
