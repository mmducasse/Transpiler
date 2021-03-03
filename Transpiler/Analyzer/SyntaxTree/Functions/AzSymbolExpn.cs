using System;
using System.Collections.Generic;
using Transpiler.Parse;
using static Transpiler.Analysis.Analyzer;

namespace Transpiler.Analysis
{
    public class AzSymbolExpn : IAzFuncExpn, IAzPattern
    {
        public IAzFuncDefn Definition { get; }
        public IAzTypeExpn Type { get; private set; }
        public CodePosition Position { get; }
        public eFixity Fixity { get; set; }

        public AzSymbolExpn(IAzFuncDefn definition,
                            IAzTypeExpn type,
                            CodePosition position,
                            eFixity fixity = eFixity.Prefix)
        {
            Definition = definition;
            Type = type;
            Position = position;
            Fixity = fixity;
        }

        public static AzSymbolExpn Analyze(Scope scope,
                                           NameProvider _,
                                           PsSymbolExpn node)
        {
            if (scope.TryGetFuncDefn(node.Name, out var funcDefn))
            {
                IAzTypeExpn type;
                // Determine the Type.
                if (funcDefn.IsSolved)
                {
                    type = funcDefn.Type.WithUniqueTvs(TypeVariables.Provider);
                }
                else if (funcDefn.Type != null)
                {
                    type = funcDefn.Type;
                }
                else if (funcDefn is IAzFuncStmtDefn stmtDefn)
                {
                    type = TypeVariables.Next;
                    stmtDefn.Type = type;
                    //throw Analyzer.Error("Function " + funcDefn.Name + " is not defined.", funcDefn.Position);
                }
                else
                {
                    throw new System.Exception();
                }

                return new(funcDefn, type, node.Position, funcDefn.Fixity);
            }

            throw Error(node.Name + " is not defined in this scope.", node.Position);
        }

        public ConstraintSet Constrain() => ConstraintSet.Empty;

        public void SubstituteType(Substitution s)
        {
            Type = Type.Substitute(s);
        }

        public void Recurse(Action<IAzFuncNode> action)
        {
            action(this);
        }

        public string Print(int i) => Definition.Name;

        public override string ToString() => Print(0);
    }
}
