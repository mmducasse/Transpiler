using System.Collections.Generic;

namespace Transpiler.Parse
{
    internal struct LexerQueue
    {
        public bool Continue { get; set; }

        public CodePosition Position { get; }
        public int Line => Position.Line;
        public int Column => Position.Column;

        private IList<string> mLines;

        public LexerQueue(Module module)
            : this(module.Code.Split('\n'),
                   true,
                   new CodePosition(module, 0, 0))
        {
        }

        private LexerQueue(IList<string> lines,
                           bool @continue,
                           CodePosition position = null)
        {
            mLines = lines;
            Continue = @continue;
            Position = position;
        }

        public char Current => mLines[Line][Column];

        public bool HasCurrent => (Line < mLines.Count) && (Column < mLines[Line].Length);
        public bool HasNext => ((Column + 1) < mLines[Line].Length);

        public LexerQueue NextCol => new LexerQueue(mLines, Continue, new CodePosition(Position.Module, Line, Column + 1));
        public LexerQueue NextRow => new LexerQueue(mLines, Continue, new CodePosition(Position.Module, Line + 1, 0));

        public override string ToString() => Current.ToString();
    }
}
