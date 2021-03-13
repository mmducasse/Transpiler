using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Transpiler.Analysis;
using Transpiler.Generate;
using Transpiler.Parse;
using static Transpiler.UI;

namespace Transpiler
{
    public class Compiler
    {
        public static Compiler Instance { get; private set; }

        private List<IAzFuncStmtDefn> mUserFunctions = new();

        public IReadOnlyDictionary<string, Module> Modules => mModules;
        public Dictionary<string, Module> mModules = new();

        private string mOutput = "";
        private bool mIsCompiled = false;

        private string mRootDir = null;

        public static bool DebugCore { get; } = false;
        public static bool DebugParser { get; } = false;
        public static bool DebugAnalyzer { get; } = false;

        public Compiler()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowSize(64, 32);
            //Console.SetBufferSize(64, 256);
#pragma warning restore CA1416 // Validate platform compatibility
            Console.WriteLine("FL1 Transpiler");
            Console.WriteLine("M. Ducasse 2021\n\n");
            Console.WriteLine("Type \"help:\" for more info.\n\n");

            Instance = this;

            new Core();

            //Process.Start("npm install prompt-sync");

            // Try to load stored root directory.
            if (File.Exists(RootDirStorageFile))
            {
                mRootDir = File.ReadAllText(RootDirStorageFile).Trim();
                PrLn("Root is now {0}", mRootDir);
            }

            // REPL Loop.
            bool doContinue = true;
            while (doContinue)
            {
                Pr("FL1> ", Blue);

                try
                {
                    string input = Console.ReadLine();
                    if (input.StartsWith("help:"))
                    {
                        Help();
                    }
                    else if (input.StartsWith("root:"))
                    {
                        SetRoot(input[5..]);
                    }
                    else if (input.StartsWith("load:"))
                    {
                        Load();
                    }
                    else if (input.StartsWith("list:"))
                    {
                        List(input[5..]);
                    }
                    else if (input.StartsWith("clear:"))
                    {
                        Clear();
                    }
                    else if (input.StartsWith("exit:"))
                    {
                        doContinue = false;
                    }
                    else if (input.StartsWith("let "))
                    {
                        AddFunction(input[3..]);
                    }
                    else if (string.IsNullOrWhiteSpace(input))
                    {
                        // Do nothing...
                    }
                    else
                    {
                        EvalExpression(input);
                    }
                }
                catch (CompilerException ce)
                {
                    ce.Print();
                }
            }
        }

        #region Commands

        private void Help()
        {

            PrLn("\nEnter an expression to evaluate...");
            PrLn("    example: 1 + 1");
            PrLn("\nOr type 'let' followed by a function definition to add it to scope...");
            PrLn("    example: let add a b = a + b");

            void PrintCmd(string cmdName, string description)
            {
                Pr("    \"");
                Pr(cmdName, foregroundColor: ConsoleColor.Yellow);
                PrLn("\"  " + description);
            }

            PrLn("\nREPL commands:");
            PrintCmd("root: <rootdir>", "sets the source root directory.");
            PrintCmd("load:", "loads all modules from source root directory into scope.");
            PrintCmd("list:", "lists all currently loaded symbols");
            PrintCmd("clear:", "unloads all currently loaded symbols.");
            PrLn("\n\n");
        }

        private void SetRoot(string rootDir)
        {
            rootDir = rootDir.Trim();
            if (string.IsNullOrWhiteSpace(rootDir))
            {
                mRootDir = null;
                PrLn("Root is now null");
                if (File.Exists(RootDirStorageFile))
                {
                    File.Delete(RootDirStorageFile);
                }
            }
            else if (Directory.Exists(rootDir))
            {
                mRootDir = rootDir;
                if (!mRootDir.EndsWith("\\"))
                {
                    mRootDir += "\\";
                }
                PrLn("Root is now {0}", mRootDir);

                File.WriteAllText(RootDirStorageFile, mRootDir);
            }
            else
            {
                PrLn("Unable to find directory: {0}", rootDir);
            }
        }

        private void Load()
        {
            Clear();

            // Load each file in directory
            //rootFolder = @"C:\Users\matth\Desktop\FunctionalCode\"; // rootFolder.Trim();
            if (mRootDir != null)
            {
                try
                {
                    mIsCompiled = false;

                    Console.Write("Loading... ");
                    var files = Directory.GetFiles(mRootDir, "*.hs", SearchOption.AllDirectories);
                    List<Module> newModules = new();
                    foreach (var file in files)
                    {
                        var module = Module.Create(file);
                        Parser.Parse(module);
                        mModules[module.Name] = module;
                        newModules.Add(module);
                    }

                    foreach (var module in newModules)
                    {
                        Analyzer.Analyze(module, new());
                    }

                    CompileModulesToJs();

                    Console.WriteLine("Ok!");

                    newModules.ForEach(m => Console.WriteLine("Loaded " + m.Name));
                }
                catch (Exception)
                {
                    Clear();
                    throw;
                }
            }
            else
            {
                Console.WriteLine("Root directory is not set.");
            }
        }

        private void Clear()
        {
            mUserFunctions.Clear();
            mModules.Clear();
        }

        private void List(string moduleName)
        {
            moduleName = moduleName.Trim();
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                ListModule("Core", Core.Instance.Scope);
                if (mUserFunctions.Count > 0)
                {
                    ListUserFunctions();
                }
                foreach (var module in Modules.Values)
                {
                    ListModule(module.Name, module.Scope);
                }
            }
            else if (Modules.TryGetValue(moduleName, out var module))
            {
                ListModule(module.Name, module.Scope);
            }
            else
            {
                throw Error("Module " + moduleName + " is not loaded.");
            }
            Console.WriteLine();
        }

        private void ListModule(string moduleName, Scope scope)
        {
            PrLn();
            PrLn(moduleName, Gray);
            //module.Scope.PrintTypes();
            scope.PrintFunctions();
        }

        private void ListUserFunctions()
        {
            PrLn();
            PrLn("USERINPUT", Gray);
            //module.Scope.PrintTypes();
            foreach (var userFunc in mUserFunctions)
            {
                userFunc.PrintSignature();
            }
        }

        private void AddFunction(string functionCode)
        {
            string inputText = "";
            foreach (var (moduleName, _) in Modules)
            {
                inputText += string.Format("use {0}\n", moduleName);
            }
            inputText += functionCode + "\n";
            var inputModule = new Module(inputText, "USERINPUT");
            foreach (var userFunc in mUserFunctions)
            {
                inputModule.Scope.AddFunction(userFunc, userFunc.Type);
            }
            Parser.Parse(inputModule);
            Analyzer.Analyze(inputModule, new());

            foreach (var userFunc in inputModule.Scope.AllFunctions())
            {
                if (!mUserFunctions.Contains(userFunc))
                {
                    mUserFunctions.Add(userFunc);
                    string fnString = string.Format("\n{0} :: {1}\n", userFunc.Name, userFunc.Type.PrintWithRefinements());
                    PrLn(fnString, Yellow);
                }
            }
        }

        private void EvalExpression(string inputExpn)
        {
            string inputText = "";
            foreach (var (moduleName, _) in Modules)
            {
                inputText += string.Format("use {0}\n", moduleName);
            }
            inputText += "ans = " + inputExpn + "\n";
            var inputModule = new Module(inputText, "USERINPUT");
            foreach (var userFunc in mUserFunctions)
            {
                inputModule.Scope.AddFunction(userFunc, userFunc.Type);
            }
            Parser.Parse(inputModule);
            Analyzer.Analyze(inputModule, new());

            var funcDefn = inputModule.Scope.AllFunctions().Where(f => !mUserFunctions.Contains(f)).First();
            string fnString = string.Format("\n{0} :: {1}", funcDefn.Name, funcDefn.Type.PrintWithRefinements());
            PrLn(fnString, Yellow);

            CompileModulesToJs();
            AddCompiledInputModule(inputModule);
            ExecOutputFile();
        }

        #endregion

        private string RootDirStorageFile
        {
            get
            {
                string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string outputPath = Directory.GetParent(execPath) + "\\root.txt";
                return outputPath;
            }
        }

        private string DestJsFile
        {
            get
            {
                string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                string outputPath = Directory.GetParent(execPath) + "\\output.js";
                return outputPath;
            }
        }

        private void CompileModulesToJs()
        {
            if (mIsCompiled) { return; }
            var output = new StringBuilder();
            output.Append(Generator.GetCoreJsCode());
            Generator.GenerateModule("Generated Core Functions", Core.Instance.Scope, ref output);

            foreach (var (_, module) in Modules)
            {
                Generator.GenerateModule(module.Name, module.Scope, ref output);
            }

            mOutput = output.ToString();
            mIsCompiled = true;
        }

        private void AddCompiledInputModule(Module inputModule)
        {
            var output = new StringBuilder();

            Generator.GenerateModule(inputModule.Name, inputModule.Scope, ref output);

            bool ansIsFunction = false;
            if (inputModule.Scope.TryGetFuncDefn("ans", out var ansDefn))
            {
                ansIsFunction = (ansDefn.Type is AzTypeLambdaExpn) || (ansDefn.Type.GetRefinements().Count > 0);
            }

            Generator.TEMP_AddFinalLine(ansIsFunction, ref output);
            File.WriteAllText(DestJsFile, mOutput);
            File.AppendAllText(DestJsFile, output.ToString());
        }

        private void ExecOutputFile()
        {
            //Console.Write("ans = ");
            var nodeJs = Process.Start("node", DestJsFile);
            nodeJs.WaitForExit();
            nodeJs.Dispose();
        }

        public static Exception Error(string reason)
        {
            return new CompilerException(eCompilerStage.Input,
                                         reason,
                                         CodePosition.Null);
        }
    }
}
