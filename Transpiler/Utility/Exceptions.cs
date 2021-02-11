using System;

namespace Transpiler
{
    public enum eInterpreterStage
    {
        Loading,
        Lexer,
        Parser,
        Analyzer,
        Generator
    }

    public class InterpreterException : Exception
    {
        private eInterpreterStage mStage;
        private string mReason = string.Empty;

        private string mModuleName = string.Empty;
        private string mCode = string.Empty;
        private CodePosition mPosition;

        public InterpreterException(eInterpreterStage stage,
                                    string reason,
                                    CodePosition position)
        {
            mStage = stage;
            mReason = reason;
            mModuleName = position.Module.Name;
            mPosition = position;

            var lines = position.Module.Code.Split("\n");
            mCode = lines[mPosition.Line];
        }

        public InterpreterException(eInterpreterStage stage,
                                    string reason)
        {
            mStage = stage;
            mReason = reason;
        }

        public string Info()
        {
            string info = "";

            info += string.Format("{0} Error in module {1}\n", StageString(mStage), mModuleName);
            info += string.Format("{0}: {1}\n", mPosition.Line + 1, mCode);
            info += mReason + "\n\n";

            return info;
        }

        private string StageString(eInterpreterStage stage)
        {
            switch (stage)
            {
                case eInterpreterStage.Loading: return "Loading";
                case eInterpreterStage.Lexer: return "Lexer";
                case eInterpreterStage.Parser: return "Parser";
                case eInterpreterStage.Analyzer: return "Analyzer";
                case eInterpreterStage.Generator: return "Generator";
            }
            return "";
        }
    }
}