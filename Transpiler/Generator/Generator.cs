using System;
using System.IO;
using System.Text;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public static class Generator
    {
        public static void Generate(Module module)
        {
            var fileScope = module.Scope;

            StringBuilder output = new();
            output.Append(GetCoreJsCode());
            Generate(CoreTypes.Instance.Scope, ref output);
            Generate(fileScope, ref output);

            TEMP_AddFinalLine(ref output);

            string destFile = @"C:\Users\matth\Desktop\output.js";
            File.WriteAllText(destFile, output.ToString());

        }

        private static void TEMP_AddFinalLine(ref StringBuilder output)
        {
            string s = "\n\nconsole.log(_ans)\n\n";
            output.Append(s);
        }

        private static void Generate(IScope scope, ref StringBuilder output)
        {
            foreach (var func in scope.FuncDefinitions.Values)
            {
                if (func is AzFuncDefn funcDefn &&
                    funcDefn.Expression != null)
                {
                    var gnFunc = GnFuncDefn.Prepare(funcDefn);
                    string g = "";
                    gnFunc.Generate(0, new(), ref g);

                    output.Append(g);
                    output.Append("\n\n");

                    Console.WriteLine();
                    Console.WriteLine(g);
                    Console.WriteLine();
                }
            }
        }

        private static string GetCoreJsCode()
        {
            string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string coreJsFile = Directory.GetParent(execPath) + "\\Generator\\Core\\Core.js";

            return File.ReadAllText(coreJsFile);
        }

        public static string Generated(this string name, int underscores = 1)
        {
            return "_".Multiply(underscores) + name;
        }

        /// <summary>
        /// Converts the string into a legal symbol name in Javascript.
        /// </summary>
        public static string SafeName(this string symbol)
        {
            string s = "";
            for (int i = 0; i < symbol.Length; i++)
            {
                s += symbol[i] switch
                {
                    '+' => "plus",
                    '-' => "minus",
                    '*' => "star",
                    '/' => "fslash",
                    '!' => "bang",
                    '=' => "equals",
                    '$' => "cash",
                    '.' => "dot",
                    '<' => "lcaret",
                    '>' => "rcaret",
                    _ => symbol[i]
                };
            }

            if (s == symbol)
            {
                return symbol.Generated();
            }

            return s;
        }
    }
}
