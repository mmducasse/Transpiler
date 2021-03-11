using System;
using static Transpiler.Extensions;
using static Transpiler.UI;

namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           string BackingFunctionName,
                           IAzTypeExpn Type,
                           eFixity Fixity = eFixity.Infix) : IAzFuncDefn
    {
        public CodePosition Position => CodePosition.Null;

        public bool IsSolved => true;

        IAzTypeExpn IAzFuncNode.Type => Type;

        public ConstraintSet Constrain()
        {
            throw new System.NotImplementedException();
        }

        public void Recurse(Action<IAzFuncNode> action) { }

        public void SubstituteType(Substitution s) { }

        public string Print(int i)
        {
            return string.Format("{0}{1} :: {2} = {3}", Indent(i), Name, Type, BackingFunctionName);
        }

        public void PrintSignature()
        {
            Pr("{0} :: ", Name);
            PrLn(Type.PrintWithRefinements(), foregroundColor: Yellow);
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
