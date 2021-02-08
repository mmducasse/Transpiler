namespace Transpiler.Parse
{
    //public interface IPsUnionTypeMember : IPsNode
    //{
    //}

    //public record PsUnionType(IReadOnlyList<IPsUnionTypeMember> SubTypes,
    //                          CodePosition Position) : IPsTypeExpn
    //{
    //    public static bool Parse(ref TokenQueue queue, out PsUnionType node)
    //    {
    //        node = null;
    //        int indent = queue.Indent;
    //        var q = queue;
    //        var p = q.Position;

    //        var q2 = q;
    //        if (!Finds(TokenType.NewLine, ref q2)) { return false; }
    //        if (!FindsIndents(ref q2, indent + 1)) { return false; }

    //        List<IPsUnionTypeMember> subTypes = new();
    //        while (Finds(TokenType.NewLine, ref q) &&
    //               FindsIndents(ref q, indent + 1) &&
    //               Finds("|", ref q))
    //        {
    //            IPsUnionTypeMember memberNode = null;

    //            if (PsTypeDefn.Parse(ref q, out var typeDefnNode)) { memberNode = typeDefnNode; }
    //            else if (PsTypeSymbol.Parse(ref q, out var symbolNode)) { memberNode = symbolNode; }

    //            if (memberNode != null)
    //            {
    //                subTypes.Add(memberNode);
    //            }
    //            else
    //            {
    //                throw Error("Expected union subtype definition or reference after '|'", q);
    //            }
    //        }

    //        node = new(subTypes, p);
    //        queue = q;
    //        return true;
    //    }

    //    public string Print(int i)
    //    {
    //        string s = "\n";
    //        int i1 = i + 1;

    //        foreach (var t in SubTypes)
    //        {
    //            s += string.Format("{0}| {1}\n", Indent(i1), t.Print(i1));
    //        }

    //        return s;
    //    }
    //}
}
