using Linq2Ldap.Core.FilterParser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Linq2Ldap.Core.Tests.FilterParser
{
    public class Lexer2254Tests
    {
        public Lexer2254 Lexer;
        public Lexer2254Tests()
        {
            Lexer = new Lexer2254();
        }

        [InlineData(@"(a=\2ab)", new[] { "(", "a", "=", "*b", ")" })]
        [Theory]
        public void Lex_UnescapesHexSequences(string input, string[] expected)
        {
            var actual = Lexer.Lex(input);
            var actualStrs = actual.Select(t => t.Text).ToArray();
            Assert.Equal(
                JsonConvert.SerializeObject(expected),
                JsonConvert.SerializeObject(actualStrs));
        }

        [InlineData(@"(a=\2a)", (char)0x2a)]
        [InlineData(@"(a=\ff)", (char)0xff)]
        [InlineData(@"(a=\00)", (char)0x00)]
        [Theory]
        public void Lex_DenotesEscapesAsStrings(string seq, char expected)
        {
            var actual = Lexer.Lex(seq).ToArray();
            Assert.False(actual[3].IsDefinedSymbol);
            Assert.Equal(new string(new char[] { expected }), actual[3].Text);
        }

        [InlineData(@"(a=\g56)", 3)]
        [InlineData(@"(a=\\)", 3)]
        [InlineData(@"(a=    \\)", 7)]
        [Theory]
        public void Lex_LegacyEscapesThrow(string seq, int pos)
        {
            var ex = Assert.Throws<FormatException>(() => Lexer.Lex(seq).ToList());
            Assert.Contains("Unescaped", ex.Message);
            Assert.Contains($"position: {pos}", ex.Message);
        }

        [InlineData(0x00)]
        [InlineData(0x5c)]
        [InlineData(0x80)]
        [InlineData(0xff)]
        [Theory]
        public void Lex_ThrowsOnOutOfRangeChars(int badCharCode)
        {
            var ex = Assert.Throws<FormatException>(
                () => Lexer.Lex(@"(a=" + (char)badCharCode + ")").ToList());
            Assert.Contains("Unescaped", ex.Message);
            Assert.Contains($"position: 3", ex.Message);
        }
    }
}
