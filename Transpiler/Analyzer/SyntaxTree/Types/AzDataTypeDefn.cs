using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public class AzDataTypeDefn : IAzTypeDefn
    {
        public string Name { get; }

        public IReadOnlyList<TypeVariable> Parameters { get; }

        public IAzTypeExpn Expression { get; set; }

        public AzFuncDefn Constructor { get; }

        public Scope Scope { get; }

        public CodePosition Position { get; }

        public AzDataTypeDefn(string name,
                              IReadOnlyList<TypeVariable> parameters,
                              AzFuncDefn constructor,
                              Scope scope,
                              CodePosition position)
        {
            Name = name;
            Parameters = parameters;
            Constructor = constructor;
            Scope = scope;
            Position = position;
        }

        public static AzDataTypeDefn Make(Scope fileScope, string name)
        {
            var ctorFunc = new AzFuncDefn(name, null, CodePosition.Null);
            var typeDefn = new AzDataTypeDefn(name, new List<TypeVariable>(), ctorFunc, fileScope, CodePosition.Null);
            typeDefn.Expression = new AzTypeTupleExpn(new List<IAzTypeExpn>(), CodePosition.Null);
            CreateConstructor(fileScope, typeDefn);
            fileScope.AddType(typeDefn);
            return typeDefn;
        }

        public static AzDataTypeDefn Initialize(Scope fileScope,
                                                PsDataTypeDefn node)
        {
            var scope = new Scope(fileScope, "Data Defn");
            var tvs = scope.AddTypeVars(node.TypeParameters);

            var ctorFunc = new AzFuncDefn(node.Name, null, node.Position);
            var typeDefn = new AzDataTypeDefn(node.Name, tvs, ctorFunc, scope, node.Position);
            fileScope.AddType(typeDefn);

            return typeDefn;
        }

        public static AzDataTypeDefn Analyze(Scope fileScope,
                                             Scope scope,
                                             AzDataTypeDefn dataType,
                                             PsDataTypeDefn node)
        {
            dataType.Expression = IAzTypeExpn.Analyze(dataType.Scope, node.Expression);
            CreateConstructor(fileScope, dataType);

            return dataType;
        }

        private static void CreateConstructor(Scope fileScope, AzDataTypeDefn dataType)
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

            var retType = new AzTypeSymbolExpn(dataType, CodePosition.Null);
            IAzTypeExpn type = retType;
            IAzFuncExpn expn = new AzGenDataExpn(dataType);

            int index = 0;
            while (argStack.TryPop(out var arg))
            {
                type = new AzTypeLambdaExpn(arg, type, CodePosition.Null);

                var argExpn = new AzParam("$" + index++, CodePosition.Null);
                expn = new AzLambdaExpn(argExpn, expn, CodePosition.Null);
            }

            var scopedExpn = new AzScopedFuncExpn(expn, new List<AzFuncDefn>(), fileScope, CodePosition.Null);

            dataType.Constructor.ScopedExpression = scopedExpn;

            Console.WriteLine("CTOR {0} :: {1}", dataType.Constructor.Print(0), type.Print(0));
            fileScope.AddFunction(dataType.Constructor, type);

        }

        public string Print(int i)
        {
            string parameters = Parameters.Select(p => p.Print()).Separate(" ", prepend: " ");
            return string.Format("{0}{1} = {2}", Name, parameters, Expression?.Print(i));
        }
    }
}
