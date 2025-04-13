namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciDivideThreadCache(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                if (cache.ContainsKey(n)) return cache[n];

                ulong k = n / 2;
                BigInt Fk = FibonacciDivideThreadCache(k), Fkp1 = FibonacciDivideThreadCache(k + 1), result;

                if ((n & 1) == 0)
                {
                    result = Fk * ( (Fkp1 << 1) - Fk);
                }
                else
                {
                    BigInt _Fkp1 = 0, _Fk = 0;

                    Task[] task =
                    [
                        Task.Run(() => { _Fkp1 = Fkp1 * Fkp1; }),
                        Task.Run(() => { _Fk = Fk * Fk; }),
                    ];
                    Task.WaitAll(task);
                    result = _Fkp1 + _Fk;
                }

                cache[n] = result;

                return result;
            }
        }
    }
}
