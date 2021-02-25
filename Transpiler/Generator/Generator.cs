using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public static class Generator
    {
        //public static void Generate(Module module)
        //{
        //    var fileScope = module.Scope;

        //    StringBuilder output = new();
        //    output.Append(GetCoreJsCode());
        //    GenerateModule("Generated Core Functions", CoreTypes.Instance.Scope, ref output);
        //    GenerateModule(module.Name, fileScope, ref output);

        //    TEMP_AddFinalLine(ref output);

        //    string destFile = @"C:\Users\matth\Desktop\output.js";
        //    File.WriteAllText(destFile, output.ToString());

        //    ExecOutputFile(destFile);
        //}

        public static void TEMP_AddFinalLine(ref StringBuilder output)
        {
            //string s = "\n\nPrintResult(_ans)\n\n";
            //s += "console.log(\"\")\n";
            //output.Append(s);
            string s = "\n\n";
            s += "try {\n";
            s += "   PrintResult(_ans)\n";
            s += "} catch (e) {\n";
            s += "   console.log(\"\")\n";
            s += "}\n";
            output.Append(s);
        }

        public static void GenerateModule(string moduleName, IScope scope, ref StringBuilder output)
        {
            output.AppendLine(string.Format("////////////////// START OF {0} //////////////////\n", moduleName));

            // Generate type classes.
            foreach (var (_, typeDefn) in scope.TypeDefinitions)
            {
                if (typeDefn is AzClassTypeDefn classDefn)
                {
                    GnClassTypeDefn.Generate(classDefn, ref output);
                }
            }

            // Generate class instances.
            foreach (var isntDefn in scope.ClassInstances)
            {
                GnClassInstDefn.Generate(scope, isntDefn, ref output);
            }

            // Generate functions.
            foreach (var func in scope.FuncDefinitions.Values)
            {
                if (func is AzFuncDefn funcDefn &&
                    funcDefn.Expression != null)
                {
                    var gnFunc = GnFuncDefn.Prepare(scope, funcDefn);
                    string g = "";
                    gnFunc.Generate(0, new("a"), ref g);

                    output.Append(g);
                    output.Append("\n\n");
                }
            }

            output.AppendLine(string.Format("////////////////// END OF {0} //////////////////\n", moduleName));
        }

        public static string GetCoreJsCode()
        {
            string execPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string coreJsFile = Directory.GetParent(execPath) + "\\Generator\\Core\\Core.js";

            return File.ReadAllText(coreJsFile);
        }

        public static string Generated(this string name, int underscores = 1)
        {
            if (string.IsNullOrEmpty(name)) { return ""; }
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
                    '?' => "qmark",
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
