using System;
using Transpiler;

Console.WriteLine("Transpiler\n");

new CoreTypes();

string path = @"C:\Users\matth\Desktop\testcode.hs";
var module = Module.Create(path);

Console.WriteLine(" === LEXER ===");
Lexer.Tokenize(module);
//Lexer.Print(module);

Console.WriteLine(" === PARSER ===");
Parser.Parse(module);
Parser.Print(module);

Console.WriteLine(" === ANALYZER ===");
Analyzer.Analyze(module);
//Analyzer.Print(module);
