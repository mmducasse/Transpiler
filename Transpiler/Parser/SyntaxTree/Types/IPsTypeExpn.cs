namespace Transpiler.Parse
{
    public interface IPsTypeExpn : IPsNode
    {
        public static bool Parse(ref TokenQueue queue, out IPsTypeExpn node)
        {
            node = null;
            var q = queue;

            //if (PsUnionType.Parse(ref q, out var unionNode)) { node = unionNode; }
            if (PsTypeLambdaExpn.Parse(ref q, out var funType)) { node = funType; }
            else if (PsTypeTupleExpn.Parse(ref q, out var tupType)) { node = tupType; }
            else if (PsTypeArbExpn.Parse(ref q, out var arbType)) { node = arbType; }

            if (node != null)
            {
                queue = q;
                return true;
            }

            return false;
        }
    }
}
