using System;
using System.Collections.Generic;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    public interface IAzTypeExpn : IAzNode
    {
        bool IsSolved { get; }

        ISet<TypeVariable> GetTypeVars();

        public static bool Equate(IAzTypeExpn a, IAzTypeExpn b)
        {
            if (a == b) { return true; }

            return (a, b) switch
            {
                (TypeVariable tva, TypeVariable tvb) =>
                    tva.Id == tvb.Id,

                (AzTypeCtorExpn ca, AzTypeCtorExpn cb) =>
                    AzTypeCtorExpn.Equate(ca, cb),

                (AzTypeLambdaExpn la, AzTypeLambdaExpn lb) =>
                    Equate(la.Input, lb.Input) && Equate(la.Output, lb.Output),

                (AzTypeTupleExpn ta, AzTypeTupleExpn tb) =>
                    AzTypeTupleExpn.Equate(ta, tb),

                // Todo: Add cases for other type expns...

                _ => false,
            };
        }

        public static IAzTypeExpn Analyze(Scope scope,
                                          IPsTypeExpn node)
        {
            return node switch
            {
                PsTypeSymbolExpn symExpn => AzTypeCtorExpn.Analyze(scope, symExpn),
                PsTypeArbExpn arbExpn => AzTypeCtorExpn.Analyze(scope, arbExpn),
                PsTypeTupleExpn tupExpn => AzTypeTupleExpn.Analyze(scope, tupExpn),
                PsTypeLambdaExpn lamExpn => AzTypeLambdaExpn.Analyze(scope, lamExpn),
                _ => throw new NotImplementedException(),
            };
        }

        public static IAzTypeExpn Substitute(IAzTypeExpn type, Substitution sub)
        {
            if (type.IsSolved) { return type; }

            if (type is TypeVariable tv &&
                sub.TypeSubstitutions.ContainsKey(tv))
            {
                return Substitute(sub.TypeSubstitutions[tv], sub);
            }

            return type switch
            {
                TypeVariable => type,
                AzTypeLambdaExpn lamType => AzTypeLambdaExpn.Substitute(lamType, sub),
                AzTypeCtorExpn ctorType => AzTypeCtorExpn.Substitute(ctorType, sub),
                AzTypeTupleExpn tupType => AzTypeTupleExpn.Substitute(tupType, sub),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
