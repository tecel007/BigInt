namespace Fibonacci
{
    partial class Program
    {

        static ulong Size(ulong n)
        {
            double phi = (1.0 + Math.Sqrt(5.0)) / 2.0;

            double len = n * Math.Log2(phi) - Math.Log2(5.0) / 2.0;

            return (ulong)Math.Ceiling(len / BITS) + 1;
        }

        unsafe static BigInt FibonacciNaiveOptimised(ulong n)
        {
            ulong size = Size(n);

            ulong[] _a = new ulong[size], _b = new ulong[size];
            ulong*[] _d = new ulong*[2];

            fixed (ulong** d = _d)
            {
                fixed (ulong* a = _a)
                {
                    fixed (ulong* b = _b)
                    {
                        //init
                        d[0] = a;
                        d[1] = b;
                        b[0] = 1;

                        ulong i = 1, j = 1;

                        while (i < n)
                        {
                            ulong* r = d[i & 1];
                            ulong* w = d[(i + 1) & 1];
                            ulong* s = r + j;

                            ulong c = 0, t;

                            while (r < s)
                            {
                                t = *r + *w + c;
                                c = (ulong)(*r > t ? 1 : 0);
                                *w = t;
                                r++; w++;
                            }
                            if (c != 0)
                            {
                                *w++ = c;
                                c = 0;
                                j++;
                            }
                            i++;
                        }
                        return new BigInt((n & 1) == 0 ? _a : _b);
                    }
                }
            }
        }
    }
}
