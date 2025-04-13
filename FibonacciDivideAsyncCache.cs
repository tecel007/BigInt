namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciDivideAsyncCache(ulong n)
        {
            BigInt result = 0;
            Task[] task =
            [
                Task.Run(() => { result = FibonacciDivideAsyncCacheAsync(n).Result; }),
            ];
            Task.WaitAll(task);

            return result;
        }

        static async Task<BigInt> FibonacciDivideAsyncCacheAsync(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                lock (cache)
                {
                    if (cache.ContainsKey(n)) return cache[n];
                }

                ulong k = n / 2;

                var _Fkp1 = FibonacciDivideAsyncCacheAsync(k + 1);
                var _Fk = FibonacciDivideAsyncCacheAsync(k);

                var Fkp1 = await _Fkp1;
                var Fk = await _Fk;

                BigInt result;

                if ((n & 1) == 0)
                {
                    result = Fk * ((Fkp1 << 1) - Fk);
                }
                else
                {
                    result = Fkp1 * Fkp1 + Fk * Fk;
                }
                
                lock(cache)
                {
                    cache[n] = result;
                }

                return result;
            }
        }
    }
}
