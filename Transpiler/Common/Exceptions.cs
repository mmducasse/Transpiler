using System;

namespace Transpiler
{
    public enum eCompilerStage
    {
        Parser,
        Analyzer,
        Generator,
        Input,
    }

    public class CompilerException : Exception
    {
        private eCompilerStage mStage;
        private string mReason = string.Empty;

        private string mModuleName = string.Empty;
        private string mCode = string.Empty;
        private CodePosition mPosition;

        public CompilerException(eCompilerStage stage,
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

        public CompilerException(eCompilerStage stage,
                                 string reason)
        {
            mStage = stage;
            mReason = reason;
        }

        public string Info
        {
            get
            {
                string info = "";

                info += string.Format("{0} Error in module {1}\n", StageString(mStage), mModuleName);
                info += string.Format("{0}: {1}\n", mPosition.Line + 1, mCode);
                info += " ".Multiply(3 + mPosition.Column) + "^\n";
                info += mReason + "\n\n";

                return info;
            }
        }

        //private static string GetAdjacentLines(string code, int line)
        //{
        //    return string.Format("{0}\n{1}\n{2}", GetLine(code, line - 1),
        //                                          GetLine(code, line),
        //                                          GetLine(code, line + 1));
        //}

        private static string GetLine(string code, int line)
        {
            var lines = code.Split("\n");
            return lines[line];
        }

        private string StageString(eCompilerStage stage)
        {
            return stage switch
            {
                eCompilerStage.Parser => "Parser",
                eCompilerStage.Analyzer => "Analyzer",
                eCompilerStage.Generator => "Generator",
                _ => "Input",
            };
        }
    }

    public static class ExceptionUtil
    {
        public static void Print(this CompilerException exception)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(exception.Info);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}