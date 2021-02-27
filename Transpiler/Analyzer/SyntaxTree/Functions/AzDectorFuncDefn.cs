﻿using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.UI;

namespace Transpiler.Analysis
{
    public class AzDectorFuncDefn : IAzFuncStmtDefn
    {
        public string ElementName { get; }

        public int ElementIndex { get; }

        public int NumElements { get; }

        public IAzTypeExpn ExplicitType { get; set; }

        public IAzTypeExpn Type { get; set; }

        public IAzFuncExpn Expression { get; set; }

        public eFixity Fixity => eFixity.Prefix;

        public CodePosition Position { get; }

        public string Name => ElementName;

        public AzDectorFuncDefn(string elementName,
                                int elementIndex,
                                int numElements,
                                CodePosition position)
        {
            ElementName = elementName;
            ElementIndex = elementIndex;
            NumElements = numElements;
            Position = position;
        }

        public static IReadOnlyList<IAzFuncStmtDefn> Initialize(Scope scope,
                                                                PsDectorFuncDefn node)
        {
            List<IAzFuncStmtDefn> dectorFuncDefns = new();
            int count = node.Elements.Count;
            for (int i = 0; i < count; i++)
            {
                string name = node.Elements[i];
                var funcDefn = new AzDectorFuncDefn(name, i, count, node.Position);
                scope.AddFunction(funcDefn);
                dectorFuncDefns.Add(funcDefn);
            }

            return dectorFuncDefns;
        }

        public static AzDectorFuncDefn Analyze(Scope parentScope,
                                               NameProvider provider,
                                               AzDectorFuncDefn funcDefn,
                                               PsDectorFuncDefn node)
        {
            var scope = new Scope(parentScope, "fn params");

            var expn = IAzFuncExpn.Analyze(scope, provider, node.Expression);

            funcDefn.Expression = expn;

            return funcDefn;
        }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            List<TypeVariable> elementTvs = new();
            for (int i = 0; i < NumElements; i++)
            {
                elementTvs.Add(provider.Next);
            }

            Type = elementTvs[ElementIndex];
            var tupType = new AzTypeTupleExpn(elementTvs, CodePosition.Null);

            var cse = Expression.Constrain(provider, scope);

            var ctup = new Constraint(tupType, Expression.Type, Position);

            return IConstraintSet.Union(cse, ctup);
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr().Concat(Expression.GetSubnodes()).ToList();
        }

        public void PrintSignature()
        {
            Pr("{0} :: ", Name);
            PrLn(ExplicitType.Print(0), foregroundColor: Yellow);
        }

        public string Print(int i)
        {
            return string.Format("{0} = {1}", Name, Expression.Print(i + 1));
        }

        public override string ToString() => Print(0);
    }
}
