namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciMultiply(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                if (cache.ContainsKey(n)) return cache[n];

                ulong k = n / 2;
                BigInt Fk = FibonacciMultiply(k), Fkp1 = FibonacciMultiply(k + 1), result;

                if ((n & 1) == 0)
                {
                    result = Multiply(Fk, (Fkp1 << 1) - Fk);
                }
                else
                {
                    result = Multiply(Fkp1, Fkp1) + Multiply(Fk, Fk);
                }
                cache[n] = result;

                return result;
            }
        }
    }
}
