namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciMultiplySchönhageStrassen(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                if (cache.ContainsKey(n)) return cache[n];

                ulong k = n / 2;
                BigInt Fk = FibonacciMultiplySchönhageStrassen(k), Fkp1 = FibonacciMultiplySchönhageStrassen(k + 1), result;

                if ((n & 1) == 0)
                {
                    result = SchönhageStrassen(Fk, (Fkp1 << 1) - Fk, true);
                }
                else
                {
                    result = SchönhageStrassen(Fkp1, true) + SchönhageStrassen(Fk, true);
                }
                cache[n] = result;

                return result;
            }
        }
    }
}
