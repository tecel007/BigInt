namespace Fibonacci
{
    partial class Program
    {
        static Tuple<BigInt, BigInt> Pair(ulong k)
        {
            if (k <= 1)
            {
                return new(k, 1);
            }
            var Fh = Pair(k >> 1);

            Tuple<BigInt, BigInt> Fk = new (Fh.Item1 * ((Fh.Item2 << 1) - Fh.Item1), Fh.Item1 * Fh.Item1 + Fh.Item2 * Fh.Item2);

            if ((k & 1) == 1)
            {
                Fk = new(Fk.Item2, Fk.Item1 + Fk.Item2);
            }
            return Fk;
        }

        static BigInt FibonacciMultiplyToomCook3Tuple(ulong n)
        {
            var r = Pair(n);

            return r.Item1;
        }
    }
}
