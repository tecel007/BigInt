namespace Fibonacci
{
    partial class Program
    {
        public unsafe static BigInt Multiply(BigInt p)
        {
            BigInt __r = new BigInt();
            BigInt temp = new BigInt([(ulong)0, 0]);

            bool sign = true;
            p = p.Sign() ? p : -p;

            p.Shrink();

            __r.Alloc((p.buffer.Length * 2) * BITS);

            for (int n = 0; n < p.buffer.Length; n++)
            {
                ulong pl = p.buffer[n] & uint.MaxValue;
                ulong ph = p.buffer[n] >> BITS_HALF;

                for (int m = n; m < p.buffer.Length; m++)
                {
                    ulong ql = p.buffer[m] & uint.MaxValue;
                    ulong qh = p.buffer[m] >> BITS_HALF;

                    if(m == n)
                    {
                        if (ph != 0 && qh != 0)
                        {
                            temp.buffer[0] = ph * qh;

                            BigInt.AddShift(__r, temp, (n + m + 1) * BITS);
                        }
                        if (ph != 0 && ql != 0)
                        {
                            temp.buffer[0] = ph * ql;

                            BigInt.AddShift(__r, temp, (n + m) * BITS + BITS_HALF + 1);
                        }
                        if (pl != 0 && ql != 0)
                        {
                            temp.buffer[0] = pl * ql;

                            BigInt.AddShift(__r, temp, (n + m) * BITS);
                        }
                    }
                    else
                    {
                        if (ph != 0 && qh != 0)
                        {
                            temp.buffer[0] = ph * qh;

                            BigInt.AddShift(__r, temp, (n + m + 1) * BITS + 1);
                        }
                        if (ph != 0 && ql != 0)
                        {
                            temp.buffer[0] = ph * ql;

                            BigInt.AddShift(__r, temp, (n + m) * BITS + BITS_HALF + 1);
                        }
                        if (pl != 0 && qh != 0)
                        {
                            temp.buffer[0] = pl * qh;

                            BigInt.AddShift(__r, temp, (n + m) * BITS + BITS_HALF + 1);
                        }
                        if (pl != 0 && ql != 0)
                        {
                            temp.buffer[0] = pl * ql;

                            BigInt.AddShift(__r, temp, (n + m) * BITS + 1);
                        }
                    }
                }
            }

            __r = sign ? __r : -__r;

            return __r;
        }

        public unsafe static BigInt Multiply(BigInt p, BigInt q)
        {
            BigInt __r = new BigInt();
            BigInt temp = new BigInt([(ulong)0, 0]);

            bool sign = p.Sign() == q.Sign();
            p = p.Sign() ? p : -p;
            q = q.Sign() ? q : -q;

            p.Shrink();
            q.Shrink();

            __r.Alloc((p.buffer.Length + q.buffer.Length) * BITS);

            for (int n = 0; n < p.buffer.Length; n++)
            {
                ulong pl = p.buffer[n] & uint.MaxValue;
                ulong ph = p.buffer[n] >> BITS_HALF;

                for (int m = 0; m < q.buffer.Length; m++)
                {
                    ulong ql = q.buffer[m] & uint.MaxValue;
                    ulong qh = q.buffer[m] >> BITS_HALF;

                    if (ph != 0 && qh != 0)
                    {
                        temp.buffer[0] = ph * qh;

                        BigInt.AddShift(__r, temp, (n + m + 1) * BITS);
                    }
                    if (ph != 0 && ql != 0)
                    {
                        temp.buffer[0] = ph * ql;

                        BigInt.AddShift(__r, temp, (n + m) * BITS + BITS_HALF);
                    }
                    if (pl != 0 && qh != 0)
                    {
                        temp.buffer[0] = pl * qh;

                        BigInt.AddShift(__r, temp, (n + m) * BITS + BITS_HALF);
                    }
                    if (pl != 0 && ql != 0)
                    {
                        temp.buffer[0] = pl * ql;

                        BigInt.AddShift(__r, temp, (n + m) * BITS);
                    }
                }
            }

            __r = sign ? __r : -__r;

            return __r;
        }
    }
}
