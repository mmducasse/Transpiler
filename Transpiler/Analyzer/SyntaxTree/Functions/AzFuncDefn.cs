using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzFuncDefn : IAzDefn, IAzFuncNode
    {
        eFixity Fixity { get; }

        IAzTypeExpn ExplicitType { get; }
    }

    // Todo: Add optional Type constraint property.
    public class AzFuncDefn : IAzFuncDefn
    {
        public string Name { get; }

        public IAzTypeExpn ExplicitType { get; }
        
        public AzScopedFuncExpn ScopedExpression { get; set; }

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
                                         AzFuncDefn funcDefn,
                                         PsFuncDefn node)
        {
            var scope = new Scope(parentScope, "fn params");

            // Turn parameters into lambdas.
            var paramStack = new Stack<AzParam>();
            foreach (var param in node.Parameters)
            {
                var paramDefn = new AzParam(param.Name, param.Position);
                scope.AddFunction(paramDefn);
                paramStack.Push(paramDefn);
            }

            var innerScopedExpn = AzScopedFuncExpn.Analyze(scope, node.ScopedExpression);
            var scopedFuncDefns = innerScopedExpn.FuncDefinitions;
            IAzFuncExpn expn = innerScopedExpn.Expression;

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

            funcDefn.ScopedExpression = new AzScopedFuncExpn(expn, scopedFuncDefns, scope, innerScopedExpn.Position);

            return funcDefn;
        }

        public static ConstraintSet Constrain(TvTable tvTable,
                                              Scope scope,
                                              AzFuncDefn node)
        {
            tvTable.AddNode(scope, node);

            var cs = AzScopedFuncExpn.Constrain(tvTable, node.ScopedExpression);

            var tf = tvTable.GetTypeOf(node);
            var te = tvTable.GetTypeOf(node.ScopedExpression.Expression);

            var c = new EqualConstraint(tf, te, node);

            return IConstraintSet.Union(c, cs);
        }

        public virtual string Print(int i)
        {
            string type = (ExplicitType == null) ? "" : " : " + ExplicitType.Print(0);
            var expn = (ScopedExpression == null) ? "" : " = " + ScopedExpression.Print(i + 1);
            return string.Format("{0}{1}{2}", Name, type, expn);
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
            string type = (ExplicitType == null) ? "" : " : " + ExplicitType.Print(0);
            return string.Format("{0}{1} = GET{2} ({3})", Name, type, TupleIndex, ScopedExpression.Print(i + 1));
        }
    }
}
