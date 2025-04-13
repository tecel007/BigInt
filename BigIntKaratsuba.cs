namespace Fibonacci
{
    partial class Program
    {
        static BigInt Karatsuba(BigInt p, bool top = false)
        {
            int n = p.Bits();

            if (n < SIZE_CUTOFF)
            {
                return Multiply(p);
            }

            n = (n + 1) / 2;

            p = p.Sign() ? p : -p;

            BigInt b = p >> n;
            BigInt a = p - (b << n);

            BigInt ac = null, bd = null, abcd = null;

            if (THREAD && top)
            {
                Task[] task =
                [
                    Task.Run(() => { ac = Karatsuba(a); }),
                    Task.Run(() => { bd = Karatsuba(b); }),
                    Task.Run(() => { abcd = Karatsuba(a + b); }),
                ];
                Task.WaitAll(task);
            }
            else
            {
                ac = Karatsuba(a);
                bd = Karatsuba(b);
                abcd = Karatsuba(a + b);
            }
            //BigInt r = ac + ((abcd - ac - bd) << n) + (bd << (n << 1));

            var __r = new BigInt();

            __r.Alloc(Math.Max(abcd.buffer.Length * BITS + n, bd.buffer.Length * BITS + (n << 1)));

            BigInt.AddShift(__r, ac, 0);
            BigInt.AddShift(__r, abcd - ac - bd, n);
            BigInt.AddShift(__r, bd, n << 1);

            __r = __r.Sign() ? __r : -__r;

            return __r;
        }

        static BigInt Karatsuba(BigInt p, BigInt q, bool top = false)
        {
            int n = Math.Max(p.Bits(), q.Bits());

            if (n < SIZE_CUTOFF)
            {
                return Multiply(p, q);
            }

            n = (n + 1) / 2;

            bool sign = p.Sign() == q.Sign();
            p = p.Sign() ? p : -p;
            q = q.Sign() ? q : -q;

            BigInt b = p >> n;
            BigInt a = p - (b << n);
            BigInt d = q >> n;
            BigInt c = q - (d << n);

            BigInt ac = null, bd = null, abcd = null;

            if (THREAD && top)
            {
                Task[] task =
                [
                    Task.Run(() => { ac = Karatsuba(a, c); }),
                    Task.Run(() => { bd = Karatsuba(b, d); }),
                    Task.Run(() => { abcd = Karatsuba(a + b, c + d); }),
                ];
                Task.WaitAll(task);
            }
            else
            {
                ac = Karatsuba(a, c);
                bd = Karatsuba(b, d);
                abcd = Karatsuba(a + b, c + d);
            }
            //BigInt r = ac + ((abcd - ac - bd) << n) + (bd << (n << 1));

            var __r = new BigInt();

            __r.Alloc(Math.Max(abcd.buffer.Length * BITS + n, bd.buffer.Length * BITS + (n << 1)));

            BigInt.AddShift(__r, ac, 0);
            BigInt.AddShift(__r, abcd - ac - bd, n);
            BigInt.AddShift(__r, bd, n << 1);

            __r = __r.Sign() ? __r : -__r;

            return __r;
        }
    }
}
