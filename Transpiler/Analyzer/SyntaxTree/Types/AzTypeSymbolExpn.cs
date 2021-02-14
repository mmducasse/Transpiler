using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Transpiler.Parse;

namespace Transpiler.Analysis
{
    //public record AzTypeSymbolExpn(IAzTypeDefn Definition,
    //                               CodePosition Position) : IAzTypeExpn
    //{
    //    public bool IsSolved => true;

    //    public static IAzTypeExpn Analyze(Scope scope,
    //                                           PsTypeSymbolExpn node)
    //    {
    //        if (scope.TryGetTypeVar(node.Name, out var tv))
    //        {
    //            return tv;
    //        }

    //        if (scope.TryGetNamedType(node.Name, out var typeDefn))
    //        {
    //            return new AzTypeSymbolExpn(typeDefn, node.Position);
    //        }

    //        throw Analyzer.Error("Type " + node.Name + " is undefined.", node.Position);
    //    }

    //    public string Print(int i)
    //    {
    //        return Definition.Name;
    //    }

    //    public override string ToString() => Print(0);
    //}
}
