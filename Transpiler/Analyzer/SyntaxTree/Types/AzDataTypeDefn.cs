using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public class AzDataTypeDefn : IAzDataTypeDefn
    {
        public string Name { get; }

        public IReadOnlyList<TypeVariable> Parameters { get; }

        public AzTypeTupleExpn Expression { get; set; }

        public AzFuncDefn Constructor { get; }

        public AzUnionTypeDefn ParentUnion { get; }

        public Scope Scope { get; }

        public CodePosition Position { get; }

        public AzDataTypeDefn(string name,
                              IReadOnlyList<TypeVariable> parameters,
                              AzFuncDefn constructor,
                              AzUnionTypeDefn parentUnion,
                              Scope scope,
                              CodePosition position)
        {
            Name = name;
            Parameters = parameters;
            Constructor = constructor;
            ParentUnion = parentUnion;
            Scope = scope;
            Position = position;
        }

        public static AzDataTypeDefn Make(Scope fileScope, string name, AzUnionTypeDefn parentUnion)
        {
            var ctorFunc = new AzFuncDefn(name, null, eFixity.Prefix, CodePosition.Null);
            var typeDefn = new AzDataTypeDefn(name, new List<TypeVariable>(), ctorFunc, parentUnion, fileScope, CodePosition.Null);

            typeDefn.Expression = new AzTypeTupleExpn(new List<IAzTypeExpn>(), CodePosition.Null);
            CreateConstructor(fileScope, typeDefn, parentUnion);
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

            var ctorFunc = new AzFuncDefn(node.Name, null, eFixity.Prefix, node.Position);
            var typeDefn = new AzDataTypeDefn(node.Name, typeVars, ctorFunc, parentUnion, scope, node.Position);
            fileScope.AddType(typeDefn);

            return typeDefn;
        }

        public static AzDataTypeDefn Analyze(Scope fileScope,
                                             AzDataTypeDefn dataType,
                                             PsDataTypeDefn node)
        {
            var typeExpn = IAzTypeExpn.Analyze(dataType.Scope, node.Expression);
            // Make sure the expression is represented as a tuple.
            if (typeExpn is AzTypeTupleExpn tupleTypeExpn)
            {
                dataType.Expression = tupleTypeExpn;
            }
            else
            {
                dataType.Expression = new AzTypeTupleExpn(typeExpn.ToArr(), node.Position);
            }
            IAzDataTypeDefn retType = (dataType.ParentUnion != null) ? dataType.ParentUnion : dataType;
            CreateConstructor(fileScope, dataType, retType);

            return dataType;
        }

        public static void CreateConstructor(Scope fileScope,
                                             AzDataTypeDefn dataType,
                                             IAzDataTypeDefn returnType)
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

            //var retType = new AzTypeSymbolExpn(dataType, CodePosition.Null);
            var retType = new AzTypeCtorExpn(returnType, returnType.Parameters, CodePosition.Null);
            IAzTypeExpn type = retType;
            AzNewDataExpn newData = new AzNewDataExpn(dataType);
            IAzFuncExpn expn = new AzScopedFuncExpn(newData, new List<AzFuncDefn>(), fileScope, CodePosition.Null);

            int index = 0;
            List<AzSymbolExpn> symbols = new();
            while (argStack.TryPop(out var arg))
            {
                type = new AzTypeLambdaExpn(arg, type, CodePosition.Null);

                var argExpn = new AzParam("$" + index++, CodePosition.Null);
                symbols.Add(new AzSymbolExpn(argExpn, CodePosition.Null));
                expn = new AzLambdaExpn(argExpn, expn, CodePosition.Null);
            }

            newData.Arguments = symbols.ToArray().Reverse().ToList();

            dataType.Constructor.Expression = expn;
            dataType.Constructor.Expression.Type = type;
            dataType.Constructor.Type = type;
            dataType.Constructor.ExplicitType = type;

            //Console.WriteLine("CTOR {0} :: {1}", dataType.Constructor.Print(0), type.Print(0));
            fileScope.AddFunction(dataType.Constructor, type);

        }

        public string Print(int i)
        {
            string parameters = Parameters.Select(p => p.Print()).Separate(" ", prepend: " ");
            return string.Format("{0}{1} = {2}", Name, parameters, Expression?.Print(i));
        }

        public override string ToString() => Name;
    }
}
