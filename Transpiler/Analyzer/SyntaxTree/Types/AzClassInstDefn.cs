using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public record AzClassInstDefn(AzClassTypeDefn Class,
                                  IAzTypeDefn Implementor,
                                  IReadOnlyList<IAzFuncDefn> Functions,
                                  CodePosition Position) : IAzDefn
    {
        public bool IsSolved => true;

        public string Name => "instance";

        public static AzClassInstDefn Initialize(Scope fileScope,
                                                 PsClassInstDefn node)
        {
            var scope = new Scope(fileScope, "Class Inst");
            var implementor = IAzTypeExpn.Analyze(scope, node.Implementor);
            if (implementor is not AzTypeCtorExpn implCtor)
            {
                throw Analyzer.Error("Invalid implementor syntax.", node.Position);
            }

            // Get a reference to the class definition.
            if (fileScope.TryGetNamedType(node.ClassName, out var typeDefn) &&
                typeDefn is AzClassTypeDefn classDefn)
            {
                // Initialize the instance's functions.
                List<AzFuncDefn> funcDefns = new();
                foreach (var funcNode in node.Functions)
                {
                    var funcDefn = AzFuncDefn.Initialize(scope, funcNode);
                    funcDefns.AddRange(funcDefn);
                }

                fileScope.AddSuperType(implCtor.TypeDefn, classDefn);
                var instDefn = new AzClassInstDefn(classDefn, implCtor.TypeDefn, funcDefns, node.Position);

                return instDefn;
            }
            else
            {
                throw Analyzer.Error("Class " + node.ClassName + " is undefined.", node.Position);
            }
        }

        //public string Print(bool terse = true)
        //{
        //    string s = string.Format("impl {0} {1}", Class.Print(0), Implementor.Print(0));
        //    if (!terse)
        //    {
        //        s += "::\n";
        //    }

        //    return s;
        //}

        public string Print(int i)
        {
            throw new NotImplementedException();
        }

        public override string ToString() => Name;
    }
}
