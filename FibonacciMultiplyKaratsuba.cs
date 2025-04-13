namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciMultiplyKaratsuba(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                if (cache.ContainsKey(n)) return cache[n];

                ulong k = n / 2;
                BigInt Fk = FibonacciMultiplyKaratsuba(k), Fkp1 = FibonacciMultiplyKaratsuba(k + 1), result;

                if ((n & 1) == 0)
                {
                    result = Karatsuba(Fk, (Fkp1 << 1) - Fk, true);
                }
                else
                {
                    result = Karatsuba(Fkp1, true) + Karatsuba(Fk, true);
                }
                cache[n] = result;

                return result;
            }
        }
    }
}
