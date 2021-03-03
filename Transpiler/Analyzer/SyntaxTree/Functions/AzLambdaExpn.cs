﻿using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.CodePosition;

namespace Transpiler.Analysis
{
    public record AzLambdaExpn(AzParam Parameter,
                               IAzFuncExpn Expression,
                               CodePosition Position) : IAzFuncExpn
    {
        public IAzTypeExpn Type { get; private set; } = TypeVariables.Next;

        public static AzLambdaExpn Make(AzParam parameter,
                                        IAzFuncExpn expression,
                                        IAzTypeExpn type,
                                        CodePosition position = null)
        {
            return new(parameter, expression, position ?? Null) { Type = type };
        }

        public static AzLambdaExpn Analyze(Scope scope,
                                           NameProvider names,
                                           PsLambdaExpn psLamExpn)
        {
            var arg = AzParam.Analyze(scope, names, psLamExpn.Parameter);
            var expr = IAzFuncExpn.Analyze(scope, names, psLamExpn.Expression);

            return new(arg, expr, psLamExpn.Position);
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            var csp = Parameter.Constrain(provider, scope);
            var cse = Expression.Constrain(provider, scope);

            var lamType = new AzTypeLambdaExpn(Parameter.Type, Expression.Type, Null);
            var cf = new Constraint(Type, lamType, Position);

            return IConstraintSet.Union(cf, csp, cse);
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Parameter.Recurse(action);
            Expression.Recurse(action);
            action(this);
        }

        public string Print(int indent)
        {
            return string.Format("{0} -> {1}",
                                 Parameter.Print(indent + 1),
                                 Expression.Print(indent + 1));
        }
    }
}
