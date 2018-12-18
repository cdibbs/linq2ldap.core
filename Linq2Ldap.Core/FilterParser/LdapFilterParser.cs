using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Linq2Ldap.Core.ExtensionMethods;
using Linq2Ldap.Core.FilterParser.Models;
using Linq2Ldap.Core.Models;
using Linq2Ldap.Core.Proxies;

namespace Linq2Ldap.Core.FilterParser
{
    public class LdapFilterParser: ILdapFilterParser
    {
        protected ILexer Lexer { get; set; }
        protected ParserOptions Options { get; set; }

        /// <summary>
        /// Creates a new LDAP filter parser.
        /// </summary>
        /// <param name="lexer">The lexer to use. Default: internal implementation.</param>
        /// <param name="options">The parsing and lexing options. Defaults include: opts.Target = RFC2254</param>
        public LdapFilterParser(
            ILexer lexer = null,
            ParserOptions options = null
        ) {
            Options = options ?? new ParserOptions();
            if (lexer != null)
            {
                lexer = Lexer;
                return;
            }

            Lexer = Options.Target == RFCTarget.RFC2254
                ? new Lexer2254(Options)
                : new Lexer(Options);
        }

        public Expression<Func<T, bool>> Parse<T>(
            string filter,
            string modelName = "m"
        )
            where T: IEntry
        {
            var tokens = Lexer.Lex(filter);
            int startPos = 0, endPos = tokens.Count() - 1;
            var argParam = Expression.Parameter(typeof(T), modelName);
            var body = _Parse<T>(tokens, startPos, endPos, argParam);
            return Expression.Lambda<Func<T, bool>>(body, argParam);
        }

        internal Expression _Parse<T>(
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            var start = tokens.ElementAt(startPos);
            var end = tokens.ElementAt(endPos);
            if (start.Text != Tokens1960.LeftParen || end.Text != Tokens1960.RightParen) {
                throw new SyntaxException("Filters are Lisp-like and must begin and end with parentheses.", startPos, endPos);
            }

            return ParseUnguarded<T>(tokens, startPos + 1, endPos - 1, paramExpr);
        }

        internal Expression ParseUnguarded<T>(
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            var op = tokens.ElementAt(startPos);
            if (new [] { Tokens1960.And, Tokens1960.Or }.Contains(op.Text)) {
                return CreateListOp<T>(op, tokens, startPos + 1, endPos, paramExpr);
            }

            if (Tokens1960.Not == op.Text) {
                return CreateNegation<T>(tokens, startPos + 1, endPos, paramExpr);
            }

            int len = endPos - startPos + 1;
            if (len == 2 && tokens.ElementAt(endPos).Text == Tokens1960.Present) {
                return CreatePresenceCheck<T>(op, tokens, startPos, endPos, paramExpr);
            }

            if (len >= 3 && OnlyNonMatchParts(tokens, startPos, endPos)) {
                return CreateSimpleCompare<T>(tokens, startPos, endPos, paramExpr);
            }

            if (len > 3 && tokens.ElementAt(startPos + 1).Text == Tokens1960.Equal) {
                return CreateMatchCheck<T>(op, tokens, startPos + 2, endPos, paramExpr);
            }

            throw new SyntaxException($"Unrecognized expression type.", startPos, endPos);
        }

        internal bool OnlyNonMatchParts(IEnumerable<Token> tokens, int startPos, int endPos)
        {
            for (var i=startPos + 2; i<=endPos; i++)
            {
                var tok = tokens.ElementAt(i);
                if (tok.IsDefinedSymbol && tok.Text == Tokens1960.Star)
                {
                    return false;
                }
            }

            return true;
        }

        internal string StringifyTokens(IEnumerable<Token> tokens, int startPos, int endPos)
        {
            return string.Join("", new ArraySegment<Token>(tokens.ToArray(), startPos, endPos - startPos + 1));
        }

        internal Expression CreatePresenceCheck<T>(
            Token left,
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            if (! left.IsDefinedSymbol) {
                var memberRef = BuildPropertyExpr<T>(left, paramExpr);
                var methodInfo = typeof(PropertyExtensions)
                    .GetMethod(
                        nameof(PropertyExtensions.Matches),
                        new [] { typeof(AttributeValueList), typeof(string) });
                return Expression.Call(methodInfo, memberRef, Expression.Constant("*"));
            }

            throw new SyntaxException($"Left side of presence check must be property name. Was a defined symbol: {left.Text}.", left.StartPos, left.EndPos);
        }

        internal Expression CreateMatchCheck<T>(
            Token left,
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            if (! left.IsDefinedSymbol) {
                var memberRef = BuildPropertyExpr<T>(left, paramExpr);
                var methodInfo = typeof(PropertyExtensions)
                    .GetMethod(
                        nameof(PropertyExtensions.Matches),
                        new [] { typeof(AttributeValueList), typeof(string) });
                var start = tokens.ElementAt(startPos);
                var matchAgg = tokens
                    .Skip(startPos + 1)
                    .Take(endPos - startPos)
                    .Aggregate(
                        new Token(start.Text, 0, start.IsDefinedSymbol),
                        AggregateOneToken);
                return Expression.Call(methodInfo, memberRef, Expression.Constant(matchAgg.Text));
            }

            throw new SyntaxException($"Invalid member reference in presence check: {left.Text}.", left.StartPos, left.EndPos);
        }

        internal Token AggregateOneToken(Token acc, Token cur) {
            if (cur.Text != Tokens1960.LeftParen)
            {
                acc.Text += cur.Text;
                acc.IsDefinedSymbol = cur.IsDefinedSymbol;
                return acc;
            }

            throw new SyntaxException($"This symbol not allowed in match expression: {cur.Text}.", cur.StartPos, cur.EndPos);
        }

        internal Expression CreateSimpleCompare<T>(
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            var left = tokens.ElementAt(startPos);
            var right = StringifyTokens(tokens, startPos + 2, endPos);
            var memberRef = BuildPropertyExpr<T>(left, paramExpr);
            var op = tokens.ElementAt(startPos + 1);
            switch(op.Text) {
                case Tokens1960.Equal:
                    return Expression.Equal(memberRef, Expression.Constant(right));
                case Tokens1960.Present:
                    return Expression.Equal(memberRef, Expression.Constant($"*{right}"));
                case Tokens1960.GTE:
                    return Expression.GreaterThanOrEqual(memberRef, Expression.Constant(right));
                case Tokens1960.LTE:
                    return Expression.LessThanOrEqual(memberRef, Expression.Constant(right));
                case Tokens1960.Approx:
                    // TODO add overload for property bag extension methods? Tests
                    // TODO flesh out method types to pass...
                    var methodInfo = typeof(PropertyExtensions)
                        .GetMethod(
                            nameof(PropertyExtensions.Approx),
                            new [] { typeof(AttributeValueList), typeof(string) });
                    return Expression.Call(methodInfo, memberRef, Expression.Constant(right));
                default:
                    throw new SyntaxException($"Unrecognized operator: {op.Text}.", op.StartPos, op.EndPos);
            }
        }

        internal Expression BuildPropertyExpr<T>(
            Token left,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            // TODO: If a field exists dedicated to this property access, use that, instead.
            var mi = typeof(T).GetProperties()
                .Where(p => p.GetIndexParameters().Any())
                .Select(p => p.GetGetMethod());
            return Expression.Call(paramExpr, mi.First(), Expression.Constant(left.Text));
        }

        internal Expression CreateListOp<T>(
            Token op,
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            if (startPos == endPos) {
                return null;
            }

            var paren = tokens.ElementAt(startPos);
            if (paren.Text == Tokens1960.RightParen) {
                // Then it was the canonical true/false that's available in some LDAP implementations
                return CanonicalBool(op);
            } else if (paren.Text != Tokens1960.LeftParen) {
                throw new SyntaxException($"Sub-filter must begin with left paren. Was: {paren.Text}.", startPos, endPos);
            }

            int closingIndex = FindClosingParenIndex(tokens, startPos, endPos);
            var subExpr = ParseUnguarded<T>(tokens, startPos + 1, closingIndex - 1, paramExpr);
            if (closingIndex != endPos) {
                var subExpr2 = CreateListOp<T>(op, tokens, closingIndex + 1, endPos, paramExpr);
                if (subExpr2 == null) {
                    return subExpr;
                }
                switch(op.MatchedToken) {
                    case Tokens1960.And: return Expression.AndAlso(subExpr, subExpr2);
                    case Tokens1960.Or:  return Expression.OrElse(subExpr, subExpr2);
                    default: throw new SyntaxException($"Unrecognized binary operation: {op.MatchedToken}.", op.StartPos, op.EndPos);
                }
            }

            return subExpr;
        }

        internal int FindClosingParenIndex(IEnumerable<Token> tokens, int startPos, int endPos) {
            int depth = 0;
            for (var i = startPos + 1; i <= endPos; i++) {
                var tok = tokens.ElementAt(i);
                if (tok.MatchedToken == Tokens1960.RightParen)
                {
                    if (depth == 0) {
                        return i;
                    }

                    depth = depth - 1;
                } else if (tok.MatchedToken == Tokens1960.LeftParen) {
                    depth = depth + 1;
                }
            }
            
            throw new SyntaxException(
                $"Mismatched left paren: {tokens.ElementAt(startPos + 1)} -> {tokens.ElementAt(endPos)}",
                startPos, endPos);
        }

        internal Expression CanonicalBool(Token token) {
            switch(token.MatchedToken) {
                case Tokens1960.And: return Expression.Constant(true);
                case Tokens1960.Or: return Expression.Constant(false);
                default:
                    throw new SyntaxException($"Unrecognized empty expression operator: {token}.", token.StartPos, token.EndPos);
            }
        }

        internal Expression CreateNegation<T>(
            IEnumerable<Token> tokens,
            int startPos,
            int endPos,
            ParameterExpression paramExpr
        )
            where T: IEntry
        {
            return Expression.Not(_Parse<T>(tokens, startPos, endPos, paramExpr));
        }
    }
}