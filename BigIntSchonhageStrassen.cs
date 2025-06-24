using System.Numerics;

namespace Fibonacci
{
    partial class Program
    {
        const int BASE = 16;

        static BigInt SchönhageStrassen(BigInt p, bool top = false)
        {
            int n = p.Bits();

            if (n < SIZE_CUTOFF)
            {
                return Multiply(p);
            }

            bool sign = true;
            p = p.Sign() ? p : -p;

            int size = GetFFTSize(BASE, p, p);
            int m = (int)Math.Log(size, 2);

            var _p = GetComplex(BASE, size, p);

            FFT(_p, m, 1);

            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] *= _p[i];
            }

            FFT(_p, m, -1);

            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] /= _p.Length;
            }

            var __r = GetBigInt(BASE, _p);

            __r = __r.Sign() == sign ? __r : -__r;

            return __r;
        }

        static BigInt SchönhageStrassen(BigInt p, BigInt q, bool top = false)
        {
            int n = Math.Max(p.Bits(), q.Bits());

            if (n < SIZE_CUTOFF)
            {
                return Multiply(p, q);
            }

            bool sign = p.Sign() == q.Sign();
            p = p.Sign() ? p : -p;
            q = q.Sign() ? q : -q;

            int size = GetFFTSize(BASE, p, q);
            int m = (int)Math.Log(size, 2);

            var _p = GetComplex(BASE, size, p);
            var _q = GetComplex(BASE, size, q);

            Task[] task =
            [
                Task.Run(() => { FFT(_p, m, 1); }),
                Task.Run(() => { FFT(_q, m, 1); }),
            ];
            Task.WaitAll(task);

            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] *= _q[i];
            }

            FFT(_p, m , -1);

            for (int i = 0; i < _p.Length; i++)
            {
                _p[i] /= _p.Length;
            }

            var __r = GetBigInt(BASE, _p);

            __r = __r.Sign() == sign ? __r : -__r;

            return __r;
        }

        public static void FFT(Complex[] buffer, int m, int sign)
        {
            int n = buffer.Length;

            // Perform bit-reversal permutation
            for (int i = 0; i < n; i++)
            {
                int j = FFTBitReverse(i, m);
                if (i < j)
                {
                    var temp = buffer[i];
                    buffer[i] = buffer[j];
                    buffer[j] = temp;
                }
            }

            // Execute the butterfly computations
            for (int size = 2; size <= n; size *= 2)
            {
                for (int i = 0; i < n; i += size)
                {
                    for (int j = 0; j < size / 2; j++)
                    {
                        Complex w = new Complex(Math.Cos(sign * 2 * j * Math.PI / size), Math.Sin(sign * 2 * j * Math.PI / size));

                        Complex u = buffer[i + j];
                        Complex t = w * buffer[i + j + size / 2];

                        buffer[i + j] += t;
                        buffer[i + j + size / 2] = u - t;
                    }
                }
            }
        }

        private static int FFTBitReverse(int x, int m)
        {
            int y = 0;

            for (int i = 0; i < m; i++)
            {
                y = (y << 1) | (x & 1);
                x >>= 1;
            }
            return y;
        }

        static int GetFFTSize(int n, BigInt p, BigInt q)
        {
            var sum = p.Bits() + q.Bits();

            var m = 1;
            while (m < sum)
            {
                m <<= 1;
            }
            return m / n;
        }

        static Complex[] GetComplex(int n, int size, BigInt p)
        {
            var r = new Complex[size];

            ulong mask = ((ulong)1 << n) - 1;

            for (int i = 0; i < r.Length; i++)
            {
                r[i] = new Complex(p & mask, 0.0);
                p >>= n;
            }
            return r;
        }

        static BigInt GetBigInt(int n, Complex[] r)
        {
            var __r = new BigInt();

            __r.Alloc(r.Length * BASE);

            for (int i = 0; i < r.Length; i++)
            {
                BigInt.AddShift(__r, (ulong)Math.Round(r[i].Real), i * BASE);
            }

            return __r;
        }
    }
}
