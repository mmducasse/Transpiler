﻿using Transpiler.Parse;
using static Transpiler.Parse.ParserUtils;

namespace Transpiler
{
    public static class Parser
    {
        public static void Parse(Module module)
        {
            var r = new ParseResult();
            var q = TokenQueue.New(module.Tokens);
            bool doContinue = true;

            while (q.HasNext && doContinue)
            {
                SkipNewlines(ref q);

                doContinue = ParseTopLevel(ref q, r);
            }

            module.ParseResult = r;
        }

        public static void Print(Module module)
        {
            module.ParseResult.Print();
        }

        private static bool ParseTopLevel(ref TokenQueue q, ParseResult r)
        {
            if (ParseModuleDefinition(ref q, r)) { return true; }
            else if (ParseImport(ref q, r)) { return true; }

            if (FuncDefnNode.Parse(ref q, out var funcNode))
            {
                System.Console.WriteLine(funcNode.Print(0));
                r.FuncDefns.Add(funcNode);
                return true;
            }
            if (TypeDefnNode.Parse(ref q, out var typeNode))
            {
                System.Console.WriteLine(typeNode.Print(0));
                r.TypeDefns.Add(typeNode);
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

            if (!Finds("use", ref q)) { return false; }
            if (!ParseModuleName(ref q, out string importName))
            {
                throw Error("Expected properly formatted module name after 'use'.", q);
            }

            r.ImportedModules.Add(importName);
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

        public static void ExpectsIndents(ref TokenQueue queue, int indent)
        {
            var q = queue;

            int i = 0;
            while (q.Current.Type == TokenType.Indent)
            {
                q = q.Next;
                i++;
            }

            if (i != indent)
            {
                Error("Expected " + indent + " indents.", queue);
            }

            queue = q;
        }

        public static bool FindsIndents(ref TokenQueue queue, int indent)
        {
            var q = queue;

            int i = 0;
            while (q.Current.Type == TokenType.Indent)
            {
                q = q.Next;
                i++;
            }

            if (i == indent)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
