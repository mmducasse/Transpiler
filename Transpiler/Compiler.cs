using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Transpiler.Analysis;
using Transpiler.Generate;
using Transpiler.Parse;

namespace Transpiler
{
    public class Compiler
    {
        public static Compiler Instance { get; private set; }

        public IReadOnlyDictionary<string, Module> Modules => mModules;
        public Dictionary<string, Module> mModules = new();

        private string mOutput = "";

        public static bool DebugCore { get; } = false;
        public static bool DebugParser { get; } = false;
        public static bool DebugAnalyzer { get; } = false;

        private const string DEST_JS_FILE = @"C:\Users\matth\Desktop\output.js";

        public Compiler()
        {
            Console.WriteLine("Transpiler");
            Console.WriteLine("M. Ducasse 2021\n\n");

            Instance = this;

            new Core();

            //string path = @"C:\Users\matth\Desktop\testcode.hs";

            // REPL Loop.
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("FL1> ");
                Console.ForegroundColor = ConsoleColor.White;

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
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n\n{0} :: {1}", funcDefn.Name, funcDefn.ExplicitType.PrintWithRefinements());
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(funcDefn.Print(0));

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
                foreach (var module in Modules.Values)
                {
                    ListModule(module);
                }
            }
            else if (Modules.TryGetValue(moduleName, out var module))
            {
                ListModule(module);
            }
            else
            {
                throw Error("Module " + moduleName + " is not loaded.");
            }
            Console.WriteLine();
        }

        private void ListModule(Module module)
        {
            Console.WriteLine();
            Console.WriteLine(module.Name);
            //module.Scope.PrintTypes();
            module.Scope.PrintFunctions();
        }

        #endregion

        private void CompileModulesToJs()
        {
            var output = new StringBuilder();
            output.Append(Generator.GetCoreJsCode());
            Generator.GenerateModule("Generated Core Functions", Core.Instance.Scope, ref output);

            foreach (var (_, module) in Modules)
            {
                Generator.GenerateModule(module.Name, module.Scope, ref output);
            }

            mOutput = output.ToString();
        }

        private void AddCompiledInputModule(Module inputModule)
        {
            var output = new StringBuilder();

            Generator.GenerateModule(inputModule.Name, inputModule.Scope, ref output);

            Generator.TEMP_AddFinalLine(ref output);
            File.WriteAllText(DEST_JS_FILE, mOutput);
            File.AppendAllText(DEST_JS_FILE, output.ToString());
        }

        private void ExecOutputFile()
        {
            Console.Write("ans = ");
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
