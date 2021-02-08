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

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }

    public interface IPsLiteralExpn<T> : IPsLiteralExpn
    {
        T Value { get; }
    }
}
