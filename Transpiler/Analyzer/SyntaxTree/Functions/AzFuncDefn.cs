using System.Collections.Generic;
using System.Linq;
using Transpiler.Parse;
using static Transpiler.UI;

namespace Transpiler.Analysis
{
    public interface IAzFuncDefn : IAzDefn, IAzFuncStmt
    {
        eFixity Fixity { get; }

        IAzTypeExpn ExplicitType { get; }
    }

    // Todo: Add optional Type constraint property.
    public class AzFuncDefn : IAzFuncDefn
    {
        public string Name { get; }

        public IAzTypeExpn ExplicitType { get; set; }

        public IAzTypeExpn Type { get; set; }

        public IAzFuncExpn Expression { get; set; }

        public eFixity Fixity { get; }

        public CodePosition Position { get; }

        public AzFuncDefn(string name,
                          IAzTypeExpn typeExpression,
                          eFixity fixity,
                          CodePosition position)
        {
            Name = name;
            ExplicitType = typeExpression;
            Fixity = fixity;
            Position = position;
        }

        public static IReadOnlyList<AzFuncDefn> Initialize(Scope scope,
                                                           PsFuncDefn node)
        {
            // Analyze the function's explicit type, if it is provided.
            IAzTypeExpn explicitType = null;
            if (node.TypeExpression != null)
            {
                explicitType = IAzTypeExpn.Analyze(scope, node.TypeExpression);
            }

            if (node.Names.Count == 1)
            {
                var funcDefn = new AzFuncDefn(node.Names[0], explicitType, node.Fixity, node.Position);
                scope.AddFunction(funcDefn);
                return funcDefn.ToArr();
            }
            else
            {
                List<AzFuncDefn> dectorFuncDefns = new();
                for (int i = 0; i < node.Names.Count; i++)
                {
                    var funcDefn = new AzDectorFuncDefn(node.Names[i], i, node.Position);
                    scope.AddFunction(funcDefn);
                    dectorFuncDefns.Add(funcDefn);
                }
                return dectorFuncDefns;
            }
        }

        public static AzFuncDefn Analyze(Scope parentScope,
                                         NameProvider provider,
                                         AzFuncDefn funcDefn,
                                         PsFuncDefn node)
        {
            var scope = new Scope(parentScope, "fn params");

            // Turn parameters into lambdas.
            var paramStack = new Stack<AzParam>();
            foreach (var param in node.Parameters)
            {
                var paramDefn = new AzParam(param.Name, false, param.Position);
                scope.AddFunction(paramDefn);
                paramStack.Push(paramDefn);
            }

            var expn = IAzFuncExpn.Analyze(scope, provider, node.Expression);

            while (paramStack.TryPop(out var paramDefn))
            {
                expn = new AzLambdaExpn(paramDefn, expn, paramDefn.Position);
            }

            //if (funcDefn is AzDectorFuncDefn dectorFuncDefn)
            //{
            //    // Todo: take expn and pack it inside getN call.

            //    funcDefn.ScopedExpression = expn;

            //    return funcDefn;
            //}

            //funcDefn.ScopedExpression = new AzScopedFuncExpn(expn, scopedFuncDefns, scope, innerScopedExpn.Position);
            funcDefn.Expression = expn;

            return funcDefn;
        }

        public ConstraintSet Constrain(TvProvider provider,
                                       Scope scope)
        {
            if (ExplicitType != null)
            {
                Type = ExplicitType.WithUniqueTvs(provider);
            }
            else if (Type == null)
            {
                Type = provider.Next;
            }

            if (Expression != null)
            {
                var cs = Expression.Constrain(provider, scope);
                var c = new Constraint(Type, Expression.Type, Position);

                return IConstraintSet.Union(c, cs);
            }

            return ConstraintSet.Empty;
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            if (Expression == null)
            {
                return this.ToArr();
            }
            return this.ToArr().Concat(Expression.GetSubnodes()).ToList();
        }

        public virtual string Print(int i)
        {
            string type = (ExplicitType == null) ? "" : " :: " + ExplicitType.Print(0);
            var expn = (Expression == null) ? "" : " = " + Expression.Print(i + 1);
            return string.Format("{0}{1}{2}", Name, type, expn);
        }

        public void PrintSignature()
        {
            Pr("{0} :: ", Name);
            PrLn(ExplicitType.Print(0), foregroundColor: Yellow);
        }
    }

    public class AzDectorFuncDefn : AzFuncDefn
    {
        public int TupleIndex { get; }

        public AzDectorFuncDefn(string name,
                                int tupleIndex,
                                CodePosition position)
            : base(name, null, eFixity.Prefix, position)
        {
            TupleIndex = tupleIndex;
        }

        public override string Print(int i)
        {
            string type = (ExplicitType == null) ? "" : " :: " + ExplicitType.Print(0);
            return string.Format("{0}{1} = GET{2} ({3})", Name, type, TupleIndex, Expression.Print(i + 1));
        }
    }
}
