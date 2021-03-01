using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public class AzDataTypeDefn : IAzDataTypeDefn
    {
        public string Name { get; }

        public IReadOnlyList<TypeVariable> Parameters { get; }

        public AzTypeTupleExpn Expression { get; set; }

        public AzFuncDefn Constructor { get; }

        public IReadOnlyList<AzFuncDefn> Accessors { get; }

        public AzUnionTypeDefn ParentUnion { get; }

        public Scope Scope { get; }

        public CodePosition Position { get; }

        public AzDataTypeDefn(string name,
                              IReadOnlyList<TypeVariable> parameters,
                              AzFuncDefn constructor,
                              IReadOnlyList<AzFuncDefn> accessors,
                              AzUnionTypeDefn parentUnion,
                              Scope scope,
                              CodePosition position)
        {
            Name = name;
            Parameters = parameters;
            Constructor = constructor;
            Accessors = accessors;
            ParentUnion = parentUnion;
            Scope = scope;
            Position = position;
        }

        public static AzDataTypeDefn Make(Scope fileScope, TvProvider tvs, string name, AzUnionTypeDefn parentUnion)
        {
            var ctorFunc = new AzFuncDefn(name, null, eFixity.Prefix, true, Null);
            var typeDefn = new AzDataTypeDefn(name,
                                              new List<TypeVariable>(),
                                              ctorFunc,
                                              new List<AzFuncDefn>(),
                                              parentUnion,
                                              fileScope,
                                              Null);

            typeDefn.Expression = new AzTypeTupleExpn(new List<IAzTypeExpn>(), Null);
            CreateConstructor(fileScope, typeDefn, parentUnion, tvs);
            fileScope.AddType(typeDefn);
            return typeDefn;
        }


        public static AzDataTypeDefn Initialize(Scope fileScope,
                                                PsDataTypeDefn node)
        {
            return Initialize(fileScope, null, node);
        }

        public static AzDataTypeDefn Initialize(Scope fileScope,
                                                AzUnionTypeDefn parentUnion,
                                                PsDataTypeDefn node)
        {
            Scope scope;
            IReadOnlyList<TypeVariable> typeVars = new List<TypeVariable>();
            if (parentUnion == null)
            {
                scope = new Scope(fileScope, "Data Defn");
                typeVars = scope.AddTypeVars(node.TypeParameters);
            }
            else if (node.TypeParameters.Count == 0)
            {
                scope = new Scope(parentUnion.Scope, "Data Defn");
            }
            else
            {
                throw Analyzer.Error("Nested data types may not have explicit type parameters", node.Position);
            }

            var ctorFunc = new AzFuncDefn(node.Name, null, eFixity.Prefix, true, node.Position);

            List<AzFuncDefn> accessors = new();
            for (int i = 0; i < node.Elements.Count; i++)
            {
                if (node.Elements[i].HasAccessor)
                {
                    var e = node.Elements[i];
                    var accessor = new AzFuncDefn(e.Name, null, eFixity.Prefix, true, e.Position);
                    accessors.Add(accessor);
                    fileScope.AddFunction(accessor);
                }
                else
                {
                    accessors.Add(null);
                }
            }

            var typeDefn = new AzDataTypeDefn(node.Name, typeVars, ctorFunc, accessors, parentUnion, scope, node.Position);
            fileScope.AddType(typeDefn);

            return typeDefn;
        }

        public static AzDataTypeDefn Analyze(Scope fileScope,
                                             TvProvider tvs,
                                             AzDataTypeDefn dataType,
                                             PsDataTypeDefn node)
        {
            List<IAzTypeExpn> elementTypes = new();
            for (int i = 0; i < node.Elements.Count; i++)
            {
                var typeExpn = IAzTypeExpn.Analyze(dataType.Scope, node.Elements[i].TypeExpression);
                elementTypes.Add(typeExpn);
            }
            dataType.Expression = new AzTypeTupleExpn(elementTypes, dataType.Position);

            IAzDataTypeDefn actualType = (dataType.ParentUnion != null) ? dataType.ParentUnion : dataType;
            for (int i = 0; i < node.Elements.Count; i++)
            {
                if (node.Elements[i].HasAccessor)
                {
                    CreateAccessor(fileScope, tvs, dataType, actualType, i, node.Elements.Count);
                }
            }

            CreateConstructor(fileScope, dataType, actualType, tvs);

            return dataType;
        }

        public static void CreateConstructor(Scope fileScope,
                                             AzDataTypeDefn dataType,
                                             IAzDataTypeDefn returnType,
                                             TvProvider tvs)
        {
            Stack<IAzTypeExpn> argStack;
            if (dataType.Expression is AzTypeTupleExpn tupleType)
            {
                argStack = new(tupleType.Elements);
            }
            else
            {
                argStack = new();
                argStack.Push(dataType.Expression);
            }

            AzTypeCtorExpn retType = new (returnType, returnType.Parameters, Null);
            IAzTypeExpn type = retType;
            AzNewDataExpn newData = new(dataType, tvs);
            IAzFuncExpn expn = new AzScopedFuncExpn(newData, new List<AzFuncDefn>(), tvs.Next, fileScope, Null);

            int index = 0;
            List<AzSymbolExpn> symbols = new();
            while (argStack.TryPop(out var arg))
            {
                type = new AzTypeLambdaExpn(arg, type, Null);

                var argExpn = new AzParam("$" + index++, IsAutoGenerated: true, tvs.Next, Null);
                symbols.Add(new(argExpn, argExpn.Type, Null));
                expn = new AzLambdaExpn(argExpn, expn, type, Null);
            }

            newData.Arguments = symbols.ToArray().Reverse().ToList();

            dataType.Constructor.Expression = expn;
            dataType.Constructor.Type = type;

            //Console.WriteLine("CTOR {0} :: {1}", dataType.Constructor.Print(0), type.Print(0));
            fileScope.AddFunction(dataType.Constructor, type);

        }

        public static void CreateAccessor(Scope fileScope,
                                          TvProvider tvs,
                                          AzDataTypeDefn dataType,
                                          IAzDataTypeDefn inputType,
                                          int elementIndex,
                                          int numElements)
        {
            var retType = dataType.Expression.Elements[elementIndex];
            var type = new AzTypeLambdaExpn(inputType.ToCtor(), retType, Null);

            var param = new AzParam("a", IsAutoGenerated: true, tvs.Next, Null);
            var symExpn = new AzSymbolExpn(param, param.Type, Null);
            var getExpn = AzGetElementExpn.Make(elementIndex, numElements, symExpn, tvs);
            var lamExpn = new AzLambdaExpn(param, getExpn, type, Null);

            var accessor = dataType.Accessors[elementIndex];
            accessor.Expression = lamExpn;
            accessor.Type = type;
        }

        public string Print(int i)
        {
            string parameters = Parameters.Select(p => p.Print()).Separate(" ", prepend: " ");
            return string.Format("{0}{1} = {2}", Name, parameters, Expression?.Print(i));
        }

        public override string ToString() => Name;

        public void PrintSignature()
        {
        }
    }
}
