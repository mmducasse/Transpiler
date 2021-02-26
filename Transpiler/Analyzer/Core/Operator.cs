using System.Collections.Generic;
using System;
using static Transpiler.Extensions;

namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           string BackingFunctionName,
                           IAzTypeExpn Type,
                           eFixity Fixity = eFixity.Infix) : IAzFuncDefn
    {
        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn ExplicitType => Type;

        IAzTypeExpn IAzFuncNode.Type { get => Type; set => throw new NotImplementedException(); }

        public ConstraintSet Constrain(TvProvider provider, Scope scope)
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<IAzFuncNode> GetSubnodes()
        {
            return this.ToArr();
        }

        public string Print(int i)
        {
            return string.Format("{0}{1} :: {2} = {3}", Indent(i), Name, Type, BackingFunctionName);
        }

        public void PrintSignature()
        {
        }

        public override string ToString() => Print(0);
    }

    public static class OperatorUtil
    {
        public static Operator Function2(string name,
                                         string backingFunctionName,
                                         IAzDataTypeDefn type,
                                         eFixity fixity = eFixity.Infix) =>
            Function2(name, backingFunctionName, type, type, type, fixity);

        public static Operator Function2(string name,
                                         string backingFunctionName,
                                         IAzDataTypeDefn arg1,
                                         IAzDataTypeDefn arg2,
                                         IAzDataTypeDefn ret,
                                         eFixity fixity = eFixity.Infix)
        {
            var type = AzTypeLambdaExpn.Make(arg1.ToCtor(),
                                             arg2.ToCtor(),
                                             ret.ToCtor());
            return new Operator(name, backingFunctionName, type, fixity);
        }
    }
}
