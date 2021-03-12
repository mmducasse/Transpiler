﻿using System;
using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.UI;

namespace Transpiler.Analysis
{
    // Todo: Add optional Type constraint property.
    public class AzFuncDefn : IAzFuncStmtDefn
    {
        public string Name { get; }

        public IAzTypeExpn Type { get; set; }

        public IAzFuncExpn Expression { get; set; }

        public eFixity Fixity { get; }

        public bool IsAutoGenerated { get; }

        public CodePosition Position { get; }

        public bool IsSolved { get; private set; }

        public bool InvokeImmediately { get; set; }

        public AzFuncDefn(string name,
                          IAzTypeExpn typeExpression,
                          eFixity fixity,
                          bool isAutoGenerated,
                          CodePosition position)
        {
            Name = name;
            Type = typeExpression;
            Fixity = fixity;
            IsAutoGenerated = isAutoGenerated;
            Position = position;
        }

        public static AzFuncDefn Initialize(Scope scope,
                                            PsFuncDefn node)
        {
            // Analyze the function's explicit type, if it is provided.
            IAzTypeExpn explicitType = null;
            if (node.TypeExpression != null)
            {
                explicitType = IAzTypeExpn.Analyze(scope, node.TypeExpression);
            }

            var funcDefn = new AzFuncDefn(node.Name, explicitType, node.Fixity, false, node.Position);
            scope.AddFunction(funcDefn);
            return funcDefn;
        }

        public static AzFuncDefn Analyze(Scope parentScope,
                                         NameProvider names,
                                         AzFuncDefn funcDefn,
                                         PsFuncDefn node)
        {
            var scope = new Scope(parentScope, "fn params");

            if (funcDefn.Type != null)
            {
                funcDefn.Type = funcDefn.Type.WithUniqueTvs(TypeVariables.Provider);
            }
            else
            {
                funcDefn.Type = TypeVariables.Next;
            }

            // Turn parameters into lambdas.
            var paramStack = new Stack<AzParam>();
            foreach (var param in node.Parameters)
            {
                var paramDefn = new AzParam(param.Name, false, param.Position);
                scope.AddFunction(paramDefn);
                paramStack.Push(paramDefn);
            }

            var expn = IAzFuncExpn.Analyze(scope, names, node.Expression);

            while (paramStack.TryPop(out var paramDefn))
            {
                expn = new AzLambdaExpn(paramDefn, expn, paramDefn.Position);
            }

            funcDefn.Expression = expn;

            return funcDefn;
        }

        public ConstraintSet Constrain()
        {
            if (Expression != null)
            {
                var cs = Expression.Constrain();
                var c = new Constraint(Type, Expression.Type, Position);

                return IConstraintSet.Union(c, cs);
            }

            return ConstraintSet.Empty;
        }

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
            IsSolved = true;
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            Expression?.Recurse(action);
            action(this);
        }

        public virtual string Print(int i)
        {
            string type = (Type == null) ? "" : " :: " + Type.Print(0);
            var expn = (Expression == null) ? "" : " = " + Expression.Print(i + 1);
            return string.Format("{0}{1}{2}", Name, type, expn);
        }

        public void PrintSignature()
        {
            Pr("{0} :: ", Name);
            if (Type is not null)
            {
                PrLn(Type.PrintWithRefinements(), foregroundColor: Yellow);
            }
        }
    }
}
