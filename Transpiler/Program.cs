﻿using System;
using Transpiler;
using Transpiler.Analysis;
using Transpiler.Parse;

Console.WriteLine("Transpiler\n");

new CoreTypes();

string path = @"C:\Users\matth\Desktop\testcode.hs";
var module = Module.Create(path);

Console.WriteLine(" === PARSER ===");
Parser.Parse(module);
Parser.Print(module);

Console.WriteLine(" === ANALYZER ===");
Analyzer.Analyze(module);
Analyzer.Print(module);