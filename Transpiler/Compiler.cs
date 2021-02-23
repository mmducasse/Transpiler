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

        public Compiler()
        {
            Console.WriteLine("Transpiler");
            Console.WriteLine("M. Ducasse 2021\n\n");

            Instance = this;

            new CoreTypes();

            //string path = @"C:\Users\matth\Desktop\testcode.hs";

            // REPL Loop.
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("FL1> ");
                Console.ForegroundColor = ConsoleColor.White;

                string input = Console.ReadLine();
                if (input.StartsWith("load:"))
                {
                    Load(input[5..]);
                }
                else if (string.IsNullOrWhiteSpace(input))
                {
                    // Do nothing...
                }
                else
                {
                    try
                    {
                        string inputText = "";
                        foreach (var (moduleName, _) in Modules)
                        {
                            inputText += string.Format("use {0}\n", moduleName);
                        }
                        inputText += "ans = " + input + "\n";
                        var inputModule = new Module(inputText, "INPUTMODULE");
                        Parser.Parse(inputModule);
                        Analyzer.Analyze(inputModule);

                        var funcDefn = inputModule.Scope.FuncDefinitions.First().Value;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("\n\n{0} :: {1}", funcDefn.Name, funcDefn.ExplicitType.PrintWithRefinements());
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine(funcDefn.Print(0));

                        var output = GenerateOutput(inputModule);

                        string TEMP_destFile = @"C:\Users\matth\Desktop\output.js";
                        ExecOutputFile(TEMP_destFile, output);
                    }
                    catch (CompilerException ce)
                    {
                        ce.Print();
                    }
                }
            }

        }

        public Module GetModule(string moduleName)
        {
            if (Modules.TryGetValue(moduleName, out var module))
            {
                if (!module.IsFinished)
                {
                    Parser.Parse(module);
                    Analyzer.Analyze(module);
                    module.IsFinished = true;
                }

                return module;
            }

            throw new Exception();
        }

        private void Load(string rootFolder)
        {
            // Load each file in directory
            rootFolder = @"C:\Users\matth\Desktop\FunctionalCode\"; // rootFolder.Trim();
            if (Directory.Exists(rootFolder))
            {
                try
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
                        Analyzer.Analyze(module);
                    }

                    Console.WriteLine("Ok!");

                    newModules.ForEach(m => Console.WriteLine("Loaded " + m.Name));
                }
                catch (CompilerException ce)
                {
                    ce.Print();
                }
            }
            else
            {
                Console.WriteLine(rootFolder + " not found.");
            }
        }

        private StringBuilder GenerateOutput(Module inputModule)
        {
            var output = new StringBuilder();
            output.Append(Generator.GetCoreJsCode());
            Generator.GenerateModule("Generated Core Functions", CoreTypes.Instance.Scope, ref output);

            foreach (var (_, module) in Modules)
            {
                Generator.GenerateModule(module.Name, module.Scope, ref output);
            }

            Generator.GenerateModule(inputModule.Name, inputModule.Scope, ref output);

            Generator.TEMP_AddFinalLine(ref output);

            return output;
        }

        private void ExecOutputFile(string filePath, StringBuilder output)
        {
            string destFile = @"C:\Users\matth\Desktop\output.js";
            File.WriteAllText(destFile, output.ToString());
            Console.Write("ans = ");
            var nodeJs = Process.Start("node", filePath);
            nodeJs.WaitForExit();
            nodeJs.Dispose();
        }
    }
}
