using System.Collections.Generic;

namespace Transpiler.Parse
{
    //public struct TokenQueue
    //{
    //    private IReadOnlyList<LexerToken> mTokens;
    //    public int Index { get; }

    //    public TokenQueue(IReadOnlyList<LexerToken> tokens, int index = 0)
    //    {
    //        mTokens = tokens;
    //        Index = index;
    //    }

    //    public LexerToken Current => mTokens[Index];

    //    public bool HasNext => (Index + 1) < mTokens.Count;
    //    public TokenQueue Next => new TokenQueue(mTokens, Index + 1);

    //    public override string ToString() => Current.Value;
    //}

    public class TokenQueue
    {
        public IReadOnlyList<LexerToken> Tokens { get; private init; }
        public int Index { get; private init; }
        public int Indent { get; private init; }

        public static TokenQueue New(IReadOnlyList<LexerToken> tokens)
        {
            return new TokenQueue
            {
                Tokens = tokens,
                Index = 0,
                Indent = 0
            };
        }

        public LexerToken Current => Tokens[Index];
        public CodePosition Position => Current.Position;

        public bool HasCurrent => Index < Tokens.Count;
        public bool HasNext => (Index + 1) < Tokens.Count;
        public TokenQueue Next
        {
            get
            {
                int indent = Current.Type switch
                {
                    TokenType.NewLine => 0,
                    TokenType.Indent => Indent + 1,
                    _ => Indent
                };

                return new TokenQueue
                {
                    Tokens = Tokens,
                    Index = Index + 1,
                    Indent = indent
                };
            }
        }

        public override string ToString() => Current.Value;
    }
}
