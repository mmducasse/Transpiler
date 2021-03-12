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

        public IReadOnlyDictionary<string, Module> Modules => mModules;
        public Dictionary<string, Module> mModules = new();

        private string mOutput = "";
        private bool mIsCompiled = false;

        public static bool DebugCore { get; } = false;
        public static bool DebugParser { get; } = false;
        public static bool DebugAnalyzer { get; } = false;

        private const string DEST_JS_FILE = @"C:\Users\matth\Desktop\output.js";

        public Compiler()
        {
#pragma warning disable CA1416 // Validate platform compatibility
            Console.SetWindowSize(64, 32);
            Console.SetBufferSize(64, 256);
#pragma warning restore CA1416 // Validate platform compatibility
            Console.WriteLine("Transpiler");
            Console.WriteLine("M. Ducasse 2021\n\n");

            Instance = this;

            new Core();

            //string path = @"C:\Users\matth\Desktop\testcode.hs";

            //Process.Start("npm install prompt-sync");

            // REPL Loop.
            while (true)
            {
                Pr("FL1> ", Blue);

                try
                {
                    string input = Console.ReadLine();
                    if (input.StartsWith("load:"))
                    {
                        Load(input[5..]);
                    }
                    else if (input.StartsWith("list:"))
                    {
                        List(input[5..]);
                    }
                    else if (string.IsNullOrWhiteSpace(input))
                    {
                        // Do nothing...
                    }
                    else
                    {
                        string inputText = "";
                        foreach (var (moduleName, _) in Modules)
                        {
                            inputText += string.Format("use {0}\n", moduleName);
                        }
                        inputText += "ans = " + input + "\n";
                        var inputModule = new Module(inputText, "INPUTMODULE");
                        Parser.Parse(inputModule);
                        Analyzer.Analyze(inputModule, new());

                        var funcDefn = inputModule.Scope.FuncDefinitions.First().Value;
                        string fnString = string.Format("\n\n{0} :: {1}", funcDefn.Name, funcDefn.Type.PrintWithRefinements());
                        PrLn(fnString, Yellow);

                        CompileModulesToJs();
                        AddCompiledInputModule(inputModule);
                        ExecOutputFile();
                    }
                }
                catch (CompilerException ce)
                {
                    ce.Print();
                }
            }
        }

        private void Load(string rootFolder)
        {
            // Load each file in directory
            rootFolder = @"C:\Users\matth\Desktop\FunctionalCode\"; // rootFolder.Trim();
            if (Directory.Exists(rootFolder))
            {
                mIsCompiled = false;

                Console.Write("Loading... ");
                var files = Directory.GetFiles(rootFolder, "*.hs", SearchOption.AllDirectories);
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
                    if (!module.IsAnalyzed)
                    {
                        Analyzer.Analyze(module, new());
                    }
                }

                CompileModulesToJs();

                Console.WriteLine("Ok!");

                newModules.ForEach(m => Console.WriteLine("Loaded " + m.Name));
            }
            else
            {
                Console.WriteLine(rootFolder + " not found.");
            }
        }

        #region List

        private void List(string moduleName)
        {
            moduleName = moduleName.Trim();
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                ListModule("Core", Core.Instance.Scope);
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

        #endregion

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
            File.WriteAllText(DEST_JS_FILE, mOutput);
            File.AppendAllText(DEST_JS_FILE, output.ToString());
        }

        private void ExecOutputFile()
        {
            //Console.Write("ans = ");
            var nodeJs = Process.Start("node", DEST_JS_FILE);
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
