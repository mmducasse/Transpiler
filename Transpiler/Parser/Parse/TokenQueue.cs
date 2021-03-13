// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using System.Collections.Generic;

namespace Transpiler.Parse
{
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
