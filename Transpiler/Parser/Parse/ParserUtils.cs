// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System;
using System.Numerics;

namespace Transpiler.Parse
{
    public static class ParserUtils
    {
        public static bool SkipNewlines(ref TokenQueue q)
        {
            while (Finds(TokenType.NewLine, ref q)) { }

            return true;
        }

        public static void Expects(char c, ref TokenQueue q)
        {
            Expects(c.ToString(), ref q);
        }

        public static void Expects(string s, ref TokenQueue q)
        {
            if (s != q.Current.Value)
                throw Error("Expected " + s + ".", q);

            q = q.Next;
        }

        public static void Expects(TokenType tokenType, ref TokenQueue q)
        {
            Expects(tokenType, ref q, out _);
        }

        public static void Expects(TokenType tokenType, ref TokenQueue q, out string value)
        {
            value = null;

            if ((tokenType & q.Current.Type) == 0)
                throw Error("Expected " + tokenType.ToString() + " token.", q);

            value = q.Current.Value;
            q = q.Next;
        }

        public static void ExpectsIndents(ref TokenQueue queue, int indent)
        {
            var q = queue;

            int i = 0;
            while (q.Current.Type == TokenType.Indent)
            {
                q = q.Next;
                i++;
            }

            if (i != indent)
            {
                Error("Expected " + indent + " indents.", queue);
            }

            queue = q;
        }

        public static bool Finds(char c, ref TokenQueue q)
        {
            return Finds(c.ToString(), ref q);
        }

        public static bool Finds(string s, ref TokenQueue q)
        {
            if ((q.HasNext) && (s == q.Current.Value))
            {
                q = q.Next;
                return true;
            }
            return false;
        }

        public static bool Finds(TokenType tokenType, ref TokenQueue q, out string value)
        {
            value = null;
            if ((q.HasNext) && ((tokenType & q.Current.Type) != 0))
            {
                value = q.Current.Value;
                q = q.Next;
                return true;
            }
            return false;
        }

        public static bool Finds(TokenType tokenType, ref TokenQueue q)
        {
            return Finds(tokenType, ref q, out _);
        }

        //public static bool FindsIndent(int indent, ref TokenQueue q)
        //{
        //    var queue = q;
        //    for(int i = 0; i < indent; i++)
        //    {
        //        if (!Finds(TokenType.Indent, ref queue)) { return false; }
        //    }

        //    q = queue;
        //    return true;
        //}

        public static bool FindsIndents(ref TokenQueue queue, int indent)
        {
            var q = queue;

            if (!q.HasCurrent) { return false; }

            int i = 0;
            while (q.Current.Type == TokenType.Indent)
            {
                q = q.Next;
                i++;
            }

            if (i == indent)
            {
                queue = q;
                return true;
            }

            return false;
        }

        public static bool ParseInteger(ref TokenQueue q, out int result, bool force = false)
        {
            result = 0;

            if (Finds(TokenType.NumberLiteral, ref q, out string num))
            {
                result = int.Parse(num);
                return true;
            }

            if (force)
                throw Error("Unable to parse integer.", q);

            return false;
        }

        public static bool ParseBigInteger(ref TokenQueue queue, out BigInteger result, bool force = false)
        {
            var q = queue;

            result = 0;
            bool isNegative = false;

            if (Finds(LexerSymbols.Minus, ref q))
            {
                isNegative = true;
            }
            if (Finds(TokenType.NumberLiteral, ref q, out string num))
            {
                result = BigInteger.Parse(num);
                result = isNegative ? -result : result;

                queue = q;
                return true;
            }

            if (force)
                throw Error("Unable to parse integer.", q);

            queue = q;
            return false;
        }

        public static string CollapseSpecialCharacters(string input)
        {
            string output = string.Empty;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == LexerSymbols.Backslash)
                {
                    i++;
                    if (i >= input.Length) { break; }

                    char c = input[i];
                    switch (input[i])
                    {
                        case 'n': c = '\n'; break;
                        case 't': c = '\t'; break;
                        case 'r': c = '\r'; break;
                        default:
                            output += '\\';
                            break;
                    }
                    output += c;
                }
                else
                {
                    output += input[i];
                }
            }

            return output;
        }


        public static Exception Error(string reason,
                                      TokenQueue queue)
        {
            return Error(reason, queue.Current.Position);
        }

        public static Exception Error(string reason,
                                      CodePosition position)
        {
            return new CompilerException(eCompilerStage.Parser,
                                            reason,
                                            position);
        }
    }
}
