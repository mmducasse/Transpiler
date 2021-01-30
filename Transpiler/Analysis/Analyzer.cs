using System;
using System.Collections.Generic;

namespace Transpiler
{
    public static class Analyzer
    {
        public static void Analyze(Module module)
        {
            // Create the top-level scope.
            var fileScope = new Scope(module, new TvTable());
            module.Scope = fileScope;

            // Find and analyze usings.

            // Analyze types in module.
            AnalyzeFileTypes(fileScope, module.ParseResult.TypeDefns);

            // Analyze functions in module.
            var newFns = AnalyzeFunctions(fileScope, module.ParseResult.FuncDefns);
            SolveFileFunctions(fileScope, newFns);

            // Apply HM type inference alg to functions
        }

        #region Analyze Types

        private static void AnalyzeFileTypes(Scope fileScope, IReadOnlyList<TypeDefnNode> typeDefns)
        {
            List<TypeDefnNode> localTypeDefns = new();

            foreach (var defn in typeDefns)
            {
                localTypeDefns.AddRange(FlattenTypeDefnNode(fileScope, defn));
            }

            foreach (var defn in localTypeDefns)
            {
                var type = AnalyzeTypeDefnNode(defn, localTypeDefns, fileScope);
                if (fileScope.TryGetType(type.Name, out _))
                {
                    throw Error("Duplicate type definition.", defn);
                }
                fileScope.Types[type.Name] = type;
            }

            foreach (var t in fileScope.Types.Values)
            {
                Console.WriteLine(t.Print());
            }
        }

        public static IReadOnlyList<TypeDefnNode> FlattenTypeDefnNode(IScope scope, TypeDefnNode typeDefn)
        {
            if (typeDefn.Expression is UnionTypeNode union)
            {
                List<TypeDefnNode> flatList = new();
                List<IUnionTypeNodeMember> newSubTypes = new();

                foreach (var t in union.SubTypes)
                {
                    if (t is TypeDefnNode defn)
                    {
                        flatList.AddRange(FlattenTypeDefnNode(scope, defn));
                        newSubTypes.Add(new TypeSymbolNode(defn.Name));
                    }
                    else if (t is TypeSymbolNode symbol &&
                             !scope.TryGetType(symbol.Name, out _))
                    {
                        flatList.Add(new TypeDefnNode(symbol.Name, new NullTypeNode()));
                        newSubTypes.Add(new TypeSymbolNode(symbol.Name));
                    }
                }

                var newUnion = new UnionTypeNode(newSubTypes);
                flatList.Add(new TypeDefnNode(typeDefn.Name, newUnion));

                return flatList;
            }
            else
            {
                return new List<TypeDefnNode> { typeDefn };
            }
        }

        public static ICompositeType AnalyzeTypeDefnNode(TypeDefnNode typeDefn,
                                                         IReadOnlyList<TypeDefnNode> localTypeDefns,
                                                         IScope scope)
        {
            if (typeDefn.Expression is NullTypeNode nullType)
            {
                return new DataType(typeDefn.Name, new List<string>());
            }
            else if (typeDefn.Expression is DataTypeNode data)
            {
                List<string> elements = new();
                foreach (var e in data.Members)
                {
                    if (!Find(e.Name))
                    {
                        throw Error("Unknown type: " + e.Name, e);
                    }
                    elements.Add(e.Name);
                }

                return new DataType(typeDefn.Name, elements);
            }
            else if (typeDefn.Expression is UnionTypeNode union)
            {
                List<string> subTypes = new();
                foreach (var t in union.SubTypes)
                {
                    var s = t as TypeSymbolNode;
                    if (!Find(s.Name))
                    {
                        throw Error("Unknown type: " + s.Name, s);
                    }
                    subTypes.Add(s.Name);
                }

                return new UnionType(typeDefn.Name, subTypes);
            }

            throw Error("Unexpected type definition.", typeDefn);

            bool Find(string typeName)
            {
                foreach (var typeDefn in localTypeDefns)
                {
                    if (typeDefn.Name == typeName)
                    {
                        return true;
                    }
                }

                return scope.TryGetType(typeName, out _);
            }
        }

        #endregion

        #region Analyze Functions

        public static IReadOnlyList<FuncDefnNode> AnalyzeFunctions(Scope scope,
                                                                   IReadOnlyList<FuncDefnNode> funcDefns)
        {
            // Register top level fn names
            // Foreach fn
            //     Make sure all symbols are defined
            //     Reorder arbitrary nodes into app nodes
            //     Solve types

            foreach (var fn in funcDefns)
            {
                if (scope.Definitions.ContainsKey(fn.Name))
                {
                    throw Error("Duplicate function definition: " + fn.Name, fn);
                }
                else
                {
                    scope.Definitions[fn.Name] = fn;
                }
            }

            List<FuncDefnNode> newFns = new();
            foreach (var fn in funcDefns)
            {
                var newScopedExpn = ScopedFuncExpnNode.Analyze(scope, fn.ScopedExpression);
                var newFn = new FuncDefnNode(fn.Name, newScopedExpn);
                scope.Definitions[fn.Name] = newFn;
                scope.TvTable.AddNode(scope, newFn);
                newFns.Add(newFn);
            }

            return newFns;
        }

        #endregion

        //public static void SolveFunctions(Scope scope,
        //                                  IReadOnlyList<FuncDefnNode> funcDefns)
        //{
        //    var tvTable = scope.TvTable;

        //    scope.TvTable.Print();

        //    for (int i = 0; i < 5; i++)
        //    {
        //        for (int j = 0; j < tvTable.Types.Count; j++)
        //        {
        //            if (tvTable.Types[j].Node is IFuncExpnNode funcExpn)
        //            {
        //                IFuncExpnNode.Solve(tvTable, funcExpn);
        //            }
        //        }
        //    }

        //    scope.TvTable.Print();
        //}

        private static void SolveFileFunctions(Scope scope,
                                               IReadOnlyList<FuncDefnNode> funcDefns)
        {
            bool progressed = true;

            scope.TvTable.Print();

            while (progressed)
            {
                progressed = SolveFunctions(scope, funcDefns);
            }

            scope.TvTable.Print();
        }

        public static bool SolveFunctions(Scope scope,
                                          IReadOnlyList<FuncDefnNode> funcDefns)
        {
            var table = scope.TvTable;

            bool progressed = false;
            foreach (var fn in funcDefns)
            {
                progressed |= ScopedFuncExpnNode.Solve(fn.ScopedExpression);
                progressed |= FuncDefnNode.Solve(table, fn);
            }

            return progressed;
        }

        public static void Print(Module module)
        {
            var scope = module.Scope as Scope;

            foreach (var fn in scope.Definitions.Values)
            {
                Console.WriteLine(fn.Print(0));
            }

            scope.TvTable.Print();
        }

        public static Exception Error(string reason,
                                      IAstNode node)
        {
            return new InterpreterException(eInterpreterStage.Analyzer,
                                            reason);
        }
    }
}
