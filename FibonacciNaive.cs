namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciNaive(ulong n)
        {
            BigInt[] d = [0, 1];

            ulong i = 1;

            while (i++ < n)
            {
                d[i & 1] += d[(i + 1) & 1];
            }
            return d[n & 1];
        }
    }
}
