namespace Transpiler.Parse
{
    public interface IPsLiteralExpn : IPsFuncExpn, IPsPattern
    {
        public static bool Parse(ref TokenQueue queue, out IPsLiteralExpn node)
        {
            node = null;
            var q = queue;

            if (PsRealLiteral.Parse(ref q, out var realNode)) { node = realNode; }
            else if (PsIntLiteral.Parse(ref q, out var intNode)) { node = intNode; }
            else if (PsCharLiteral.Parse(ref q, out var charNode)) { node = charNode; }
            else if (PsStringLiteral.Parse(ref q, out var stringNode)) { node = stringNode; }
            else if (PsListLiteral.Parse(ref q, out var listNode)) { node = listNode; }
            else if (PsUndefinedLiteral.Parse(ref q, out var undefNode)) { node = undefNode; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
