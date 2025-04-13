namespace Fibonacci
{
    partial class Program
    {
        static BigInt FibonacciDivideThread(ulong n)
        {
            if (n < (ulong)small.Length)
            {
                return small[n];
            }
            else
            {
                ulong k = n / 2;
                BigInt Fk = 0, Fkp1 = 0, result;

                Task[] task =
                [
                    Task.Run(() => { Fk = FibonacciDivideThread(k); }),
                    Task.Run(() => { Fkp1 = FibonacciDivideThread(k + 1); }),
                ];
                Task.WaitAll(task);

                if ((n & 1) == 0)
                {
                    result = Fk * ((Fkp1 << 1) - Fk);
                }
                else
                {
                    result = Fkp1 * Fkp1 + Fk * Fk;
                }
                
                return result;
            }
        }
    }
}
