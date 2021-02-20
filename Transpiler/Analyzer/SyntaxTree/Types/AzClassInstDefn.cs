using System;
using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record AzClassInstDefn(AzClassTypeDefn Class,
                                  IAzTypeDefn Implementor,
                                  IReadOnlyList<TypeVariable> TypeParameters,
                                  IReadOnlyList<IAzFuncDefn> Functions,
                                  CodePosition Position) : IAzDefn
    {
        public bool IsSolved => true;

        public string Name => "instance";

        public static AzClassInstDefn Initialize(Scope fileScope,
                                                 PsClassInstDefn node)
        {
            var scope = new Scope(fileScope, "Class Inst");

            // Get a reference to the implementor type definition.
            if (fileScope.TryGetNamedType(node.ImplementorName, out var implType) &&
                implType is IAzDataTypeDefn implTypeDefn)
            {
                // Verify that there are the correct number of type parameters.
                if (node.TypeParameters.Count != implTypeDefn.Parameters.Count)
                {
                    throw Analyzer.Error("Incorrect number of type parameters for type " + node.ImplementorName, node.Position);
                }

                List<TypeVariable> typeParams = new();
                foreach (var tvName in node.TypeParameters)
                {
                    var tv = scope.AddTypeVar(tvName);
                    typeParams.Add(tv);
                }

                // Get a reference to the class definition.
                if (fileScope.TryGetNamedType(node.ClassName, out var typeDefn) &&
                    typeDefn is AzClassTypeDefn classDefn)
                {
                    var instSub = new Substitution(classDefn.TypeVar, implTypeDefn.ToCtor());
                    
                    // Initialize the instance's functions.
                    Dictionary<AzFuncDefn, AzFuncDefn> funcDefns = new();
                    foreach (var funcNode in node.Functions)
                    {
                        var fnDefns = AzFuncDefn.Initialize(scope, funcNode);
                        if (fnDefns.Count != 1)
                        {
                            throw Analyzer.Error("Class functions may not have tuple deconstructors.", node.Position);
                        }
                        var funcDefn = fnDefns[0];
                        if (!classDefn.TryGetFunction(funcDefn.Name, out var classFuncDefn))
                        {
                            throw Analyzer.Error("Instance function " + funcDefn.Name + " is undefined.", node.Position);
                        }
                        if (funcDefns.ContainsKey(classFuncDefn))
                        {
                            throw Analyzer.Error("Duplicate nstance function " + funcDefn.Name + ".", node.Position);
                        }
                        var explicitType = classFuncDefn.ExplicitType.Substitute(instSub);
                        funcDefn.ExplicitType = explicitType;
                        funcDefns[classFuncDefn] = funcDefn;
                    }

                    List<AzFuncDefn> orderedFuncDefns = new();
                    foreach (var classFuncDefn in classDefn.Functions)
                    {
                        if (!funcDefns.ContainsKey(classFuncDefn))
                        {
                            throw Analyzer.Error("Instance is missing function " + classFuncDefn.Name + ".", node.Position);
                        }
                        orderedFuncDefns.Add(funcDefns[classFuncDefn]);
                    }

                    //fileScope.AddSuperType(implTypeDefn, classDefn);
                    var instDefn = new AzClassInstDefn(classDefn, implTypeDefn, typeParams, orderedFuncDefns, node.Position);
                    fileScope.AddClassInstance(instDefn);

                    return instDefn;
                }
                else
                {
                    throw Analyzer.Error("Class " + node.ClassName + " is undefined.", node.Position);
                }
            }
            else
            {
                throw Analyzer.Error("Implementor type " + node.ImplementorName + " is undefined.", node.Position);
            }
        }

        public static AzClassInstDefn Analyze(Scope scope,
                                              AzClassInstDefn classInstDefn,
                                              PsClassInstDefn node)
        {
            foreach (var funcDefn in classInstDefn.Functions)
            {
                foreach (var funcsNode in node.Functions)
                {
                    if (funcDefn.Name == funcsNode.Names[0])
                    {
                        AzFuncDefn.Analyze(scope, funcDefn as AzFuncDefn, funcsNode);
                    }
                }
            }

            return classInstDefn;
        }

        public string Print(int i)
        {
            string s = string.Format("inst {0} {1} =\n", Class.Name, Implementor.Name);
            foreach (var instFunc in Functions)
            {
                s += string.Format("{0}{1}\n", Indent(i + 1), instFunc.Print(0));
            }
            return s;
        }

        public override string ToString() => Name;
    }
}
