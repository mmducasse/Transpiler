// //////////////////////////////////////////// //
//                                              //
// Project: Functional Language 1 Transpiler    //
// Author:  Matthew M. Ducasse 2021             //
//                                              //
// //////////////////////////////////////////// //

using static Transpiler.Parse.ParserUtils;

namespace Transpiler.Parse
{
    public static class Parser
    {
        public static void Parse(Module module)
        {
            Lexer.Tokenize(module);
            //Lexer.Print(module);

            var r = new ParseResult();
            var q = TokenQueue.New(module.Tokens);
            bool doContinue = true;

            while (q.HasNext && doContinue)
            {
                SkipNewlines(ref q);

                doContinue = ParseTopLevel(ref q, r);
            }

            module.ParseResult = r;

            if (r.ModuleName == null && module.Name == null)
            {
                throw Error("Module name is missing in file.", CodePosition.Zero(module));
            }
            module.Name = r.ModuleName;

            if (Compiler.DebugParser)
            {
                Print(module);
            }
        }

        public static void Print(Module module)
        {
            module.ParseResult.Print();
        }

        private static bool ParseTopLevel(ref TokenQueue q, ParseResult r)
        {
            if (ParseModuleDefinition(ref q, r)) { return true; }
            else if (ParseImport(ref q, r)) { return true; }

            if (PsClassInstDefn.Parse(ref q, out var instNode))
            {
                r.InstDefns.Add(instNode);
                return true;
            }
            if (PsClassTypeDefn.Parse(ref q, out var classNode))
            {
                r.ClassDefns.Add(classNode);
                return true;
            }
            if (IPsTypeDefn.Parse(ref q, allowClasses: false, out var typeNode))
            {
                r.TypeDefns.Add(typeNode);
                return true;
            }
            if (IPsFuncStmtDefn.Parse(ref q, out var funcStmtNode))
            {
                r.FuncDefns.Add(funcStmtNode);
                return true;
            }

            return false;
        }

        private static bool ParseModuleDefinition(ref TokenQueue queue, ParseResult r)
        {
            var q = queue;

            if (!Finds("mod", ref q)) { return false; }
            if (!ParseModuleName(ref q, out string moduleName))
            {
                throw Error("Expected properly formatted module name after 'module'.", q);
            }

            if (r.ModuleName != null)
            {
                throw Error("Module must only have one name definition.", q);
            }

            r.ModuleName = moduleName;
            queue = q;
            return true;
        }

        private static bool ParseImport(ref TokenQueue queue, ParseResult r)
        {
            var q = queue;
            var p = q.Position;

            if (!Finds("use", ref q)) { return false; }
            if (!ParseModuleName(ref q, out string importName))
            {
                throw Error("Expected properly formatted module name after 'use'.", q);
            }

            r.ImportedModules.Add(new(importName, p));
            queue = q;
            return true;
        }

        private static bool ParseModuleName(ref TokenQueue queue, out string moduleName)
        {
            moduleName = "";
            var q = queue;

            if (!Finds(TokenType.Uppercase, ref q, out string firstSegment)) { return false; }
            moduleName += firstSegment;

            while (Finds(".", ref q))
            {
                Expects(TokenType.Uppercase, ref q, out string nextSegment);
                moduleName += ("." + nextSegment);
            }

            Expects(TokenType.NewLine, ref q);

            queue = q;
            return true;
        }
    }
}
