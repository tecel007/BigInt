namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciDivideCache(ulong n)
        {
            if(n < (ulong) small.Length)
            {
                return small[n];
            }
            else
            {
                if (cache.ContainsKey(n)) return cache[n];

                ulong k = n / 2;
                BigInt Fk = FibonacciDivideCache(k), Fkp1 = FibonacciDivideCache(k + 1), result;

                if ((n & 1) == 0)
                {
                    result = Fk * ((Fkp1 << 1) - Fk);
                }
                else
                {
                    result = Fkp1 * Fkp1 + Fk * Fk;
                }
                cache[n] = result;

                return result;
            }
        }
    }
}
