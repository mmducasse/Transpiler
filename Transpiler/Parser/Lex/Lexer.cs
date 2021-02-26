using System.Collections.Generic;
using System.Linq;

namespace Transpiler.Parse
{
    public static class Lexer
    {
        private class TokenList
        {
            public IReadOnlyList<LexerToken> Tokens => mTokens;
            private List<LexerToken> mTokens = new List<LexerToken>();

            public void Add(TokenType type, CodePosition position, string value = "")
            {
                var token = new LexerToken(type);
                token.Value = value;
                token.Position = position;
                mTokens.Add(token);
            }
        }

        public const string ExitSymbol = "@exit";

        private static bool Match(ref LexerQueue queue, string s, out CodePosition position)
        {
            var q = queue;
            position = q.Position;
            int i = 0;
            while (i < s.Length)
            {
                if (!Match(ref q, s[i++], out _)) { return false; }
            }

            queue = q;
            return true;
        }

        private static bool Match(ref LexerQueue q, char c, out CodePosition position)
        {
            position = q.Position;
            if (q.HasCurrent && (q.Current == c))
            {
                q = q.NextCol;
                return true;
            }

            return false;
        }

        public static void Tokenize(Module module)
        {
            var q = new LexerQueue(module);
            var tokens = new TokenList();

            while (q.Continue && q.HasCurrent)
            {
                TokenizeLine(ref q, tokens);
            }

            module.Tokens = tokens.Tokens;
        }

        public static void Print(Module module)
        {
            foreach (var t in module.Tokens)
            {
                System.Console.WriteLine(t);
            }
        }

        private static void TokenizeLine(ref LexerQueue q,
                                         TokenList tokens)
        {
            while (q.Continue && q.HasCurrent)
            {
                TokenizeIndents(ref q, tokens);
                TokenizeNext(ref q, tokens);
            }

            tokens.Add(TokenType.NewLine, q.Position);
            q = q.NextRow;
        }

        private static void TokenizeIndents(ref LexerQueue q, TokenList tokens)
        {
            while (Match(ref q, LexerSymbols.Tab, out var position))
            {
                tokens.Add(TokenType.Indent, position);
            }
        }

        private static void TokenizeNext(ref LexerQueue q,
                                         TokenList tokens)
        {
            SkipSpaces(ref q);
            if (!q.HasCurrent) { return; }

            if (MatchExitString(ref q)) { return; }
            if (MatchComment(ref q, tokens)) { return; }
            if (MatchNumberLiteral(ref q, tokens)) { return; }
            if (MatchAlphabetic(ref q, tokens)) { return; }
            if (MatchSingleQuoted(ref q, tokens)) { return; }
            if (MatchDoubleQuoted(ref q, tokens)) { return; }
            if (MatchKeySymbol(ref q, tokens)) { return; }
            if (MatchSymbolic(ref q, tokens)) { return; }

            Error("Unexpected character.", q.Position);
        }

        private static void SkipSpaces(ref LexerQueue q)
        {
            while (Match(ref q, LexerSymbols.Space, out _)
                   || Match(ref q, LexerSymbols.Tab, out _)
                   || Match(ref q, LexerSymbols.ReturnCIReadOnlyListiage, out _))
            { }
        }

        private static bool MatchExitString(ref LexerQueue q)
        {
            if (Match(ref q, ExitSymbol, out _))
            {
                q.Continue = false;
                return true;
            }
            return false;
        }

        private static bool MatchComment(ref LexerQueue q,
                                         TokenList tokens)
        {
            if (Match(ref q, "--", out var position))
            {
                q = q.NextRow;

                tokens.Add(TokenType.NewLine, position);
                return true;
            }

            return false;
        }

        private static bool MatchNumberLiteral(ref LexerQueue q, TokenList tokens)
        {
            bool isPositive = true;
            var position = q.Position;
            var q2 = q;
            if (q2.Current == '-')
            {
                isPositive = false;
                q2 = q.NextCol;
            }
            if (!char.IsNumber(q2.Current)) { return false; }
            q = q2;

            string number = isPositive ? string.Empty : "-";

            while (true)
            {
                if (Match(ref q, LexerSymbols.Underscore, out _))
                {
                    continue;
                }
                else if (char.IsNumber(q.Current))
                {
                    number += q.Current;
                    q = q.NextCol;
                    if (!q.HasCurrent) { break; }
                }
                else if (char.IsLetter(q.Current))
                {
                    Error("Identifiers cannot start with a digit.", position);
                }
                else
                {
                    break;
                }
            }

            tokens.Add(TokenType.NumberLiteral, position, number);

            return true;
        }

        private static bool MatchAlphabetic(ref LexerQueue q, TokenList tokens)
        {
            TokenType type = TokenType.Uppercase;
            if (!char.IsUpper(q.Current))
            {
                type = TokenType.Lowercase;
                if (!char.IsLower(q.Current)
                    && (q.Current != LexerSymbols.Underscore))
                {
                    return false;
                }
            }

            var position = q.Position;
            string alpha = "";

            while (true)
            {
                if (!q.HasCurrent) { break; }

                if (char.IsLetterOrDigit(q.Current)
                    || (q.Current == LexerSymbols.Underscore))
                {
                    alpha += q.Current;
                    q = q.NextCol;
                }
                else
                {
                    break;
                }
            }

            tokens.Add(type, position, alpha);
            return true;
        }

        private static bool MatchSingleQuoted(ref LexerQueue q, TokenList tokens)
        {
            if (!Match(ref q, LexerSymbols.SingleQuote, out var position)) { return false; }

            if (!q.HasCurrent) { Error("Unterminated single quotes.", position); }
            if (q.Current == LexerSymbols.SingleQuote) { Error("Single quotes must contain exactly one character.", position); }

            string literal = "";
            while (true)
            {
                if (!q.HasCurrent) { Error("Unterminated single quotes.", position); }
                if (Match(ref q, LexerSymbols.SingleQuote, out _)) { break; }

                literal += q.Current;
                q = q.NextCol;
            }

            literal = ParserUtils.CollapseSpecialCharacters(literal);

            if (literal.Length != 1) { Error("Single quotes must contain exactly one character.", position); }

            tokens.Add(TokenType.SingleQuoted, position, literal);

            return true;
        }

        private static bool MatchDoubleQuoted(ref LexerQueue q, TokenList tokens)
        {
            if (!Match(ref q, LexerSymbols.DoubleQuote, out var position)) { return false; }
            if (!q.HasCurrent) { Error("Unterminated double quotes.", position); }

            string literal = "";
            while (!Match(ref q, LexerSymbols.DoubleQuote, out _))
            {
                literal += q.Current;
                q = q.NextCol;

                if (!q.HasCurrent) { Error("Unterminated double quotes.", position); }
            }

            literal = ParserUtils.CollapseSpecialCharacters(literal);

            tokens.Add(TokenType.DoubleQuoted, position, literal);

            return true;
        }

        private static bool MatchKeySymbol(ref LexerQueue q, TokenList tokens)
        {
            var position = q.Position;
            string symbol = q.Current.ToString();

            if (KeySymbols.All.Contains(symbol))
            {
                tokens.Add(TokenType.KeySymbol, position, symbol);
                q = q.NextCol;
                return true;
            }

            return false;
        }

        private static bool MatchSymbolic(ref LexerQueue q, TokenList tokens)
        {
            var position = q.Position;
            string symbol = "";

            if (!char.IsWhiteSpace(q.Current)
                && !char.IsLetterOrDigit(q.Current))
            {
                while (q.HasCurrent &&
                       !char.IsWhiteSpace(q.Current)
                       && !char.IsLetterOrDigit(q.Current)
                       && !KeySymbols.All.Contains(q.Current.ToString()))
                {
                    symbol += q.Current;
                    q = q.NextCol;
                }

                if (KeySymbols.ReservedSymbols.Contains(symbol))
                {
                    tokens.Add(TokenType.KeySymbol, position, symbol);
                }
                else
                {
                    tokens.Add(TokenType.Symbol, position, symbol);
                }
                return true;
            }

            return false;
        }

        private static void Error(string reason, CodePosition position)
        {
            throw new CompilerException(eCompilerStage.Parser,
                                        reason,
                                        position);
        }
    }
}