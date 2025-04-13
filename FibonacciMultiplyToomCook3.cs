namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciMultiplyToomCook3(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                if (cache.ContainsKey(n)) return cache[n];

                ulong k = n / 2;
                BigInt Fk = FibonacciMultiplyToomCook3(k), Fkp1 = FibonacciMultiplyToomCook3(k + 1), result;

                if ((n & 1) == 0)
                {
                    result = ToomCook3(Fk, (Fkp1 << 1) - Fk, true);
                }
                else
                {
                    result = ToomCook3(Fkp1, true) + ToomCook3(Fk, true);
                }
                cache[n] = result;

                return result;
            }
        }
    }
}
