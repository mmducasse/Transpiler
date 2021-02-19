using System.Collections.Generic;

namespace Transpiler.Analysis
{
    public record Operator(string Name,
                           string BackingFunctionName,
                           IAzTypeExpn Type,
                           eFixity Fixity = eFixity.Infix) : IAzFuncDefn
    {
        public CodePosition Position => CodePosition.Null;

        public IAzTypeExpn ExplicitType => Type;

        public string Print(int i) => Name;

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

        public static void AddInstFunc2(Dictionary<AzFuncDefn, IAzFuncDefn> dictionary,
                                        AzClassTypeDefn classDefn,
                                        string funcName,
                                        string backingFuncName,
                                        IAzTypeExpn type,
                                        eFixity fixity = eFixity.Infix)
        {
            var op = new Operator(funcName, backingFuncName, type, fixity);

            if (!classDefn.TryGetFunction(op.Name, out var classFuncDefn))
            {
                throw new System.Exception();
            }

            dictionary[classFuncDefn] = op;
        }
    }
}
