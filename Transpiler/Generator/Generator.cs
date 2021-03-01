using System.IO;
using System.Text;
using Transpiler.Analysis;

namespace Transpiler.Generate
{
    public static class Generator
    {
        public static void TEMP_AddFinalLine(ref StringBuilder output)
        {
            string s = "\n\n";
            s += "try {\n";
            s += "   PrintResult(_ans())\n";
            s += "   console.log(\"\")\n";
            s += "} catch (e) {\n";
            s += "   console.log(e)\n";
            s += "   console.log(\"\")\n";
            s += "}\n";
            output.Append(s);
        }

        public static void GenerateModule(string moduleName, Scope scope, ref StringBuilder output)
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
                string g = "";
                if (func is IAzFuncStmtDefn funcDefn &&
                    funcDefn.Expression != null)
                {
                    var gnFunc = IGnFuncStmtDefn.Prepare(scope, funcDefn);
                    gnFunc.Generate(0, new("a"), ref g);

                    output.Append(g);
                    output.Append("\n\n");
                }
                else if (func is Operator op)
                {
                    GnOperator.Generate(op, 0, ref g);

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

            return s;
        }

        public static string SafeNameGenerated(this string symbol)
        {
            string result = symbol.SafeName();
            if (result == symbol)
            {
                return result.Generated();
            }
            return result;
        }
    }
}
