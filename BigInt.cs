//#define TEST

using System.Numerics;

namespace Fibonacci
{
    partial class Program
    {
        public static bool THREAD = true;
        public const bool SHRINK = true;

        public const int SIZE_CUTOFF = BITS * 16;

        public const int BYTES = sizeof(ulong);
        public const int BITS = 8 * BYTES;
        public const int BITS_HALF = BITS / 2;

        public unsafe class BigInt
        {
            public ulong[] buffer;

            public BigInt()
            {
                buffer = [0];
            }

            public BigInt(ulong[] _buffer)
            {
                buffer = _buffer;
            }

            public static implicit operator BigInt(uint p)
            {
                return new BigInt([p]);
            }

            public static implicit operator BigInt(ulong p)
            {
                return p >> (BITS - 1) == 0 ? new BigInt([p]) : new BigInt([p, 0]);
            }

            public static implicit operator BigInt(int p)
            {
                return new BigInt([(ulong)(long)p]);
            }

            public static implicit operator BigInt(long p)
            {
                return new BigInt([(ulong)p]);
            }

            public BigInt(byte[] p)
            {
                int n = (p.Length + BYTES - 1) / BYTES;

                buffer = new ulong[n];

                Buffer.BlockCopy(p, 0, buffer, 0, p.Length);
            }

            public static implicit operator BigInteger(BigInt p)
            {
                var buffer = new byte[p.buffer.Length * BYTES];

                Buffer.BlockCopy(p.buffer, 0, buffer, 0, buffer.Length);

                return new BigInteger(buffer);
            }

            public static implicit operator BigInt(BigInteger p)
            {
                return new BigInt(p.ToByteArray());
            }

            public void Shrink()
            {
                if (SHRINK)
                {
                    int zero = 0;

                    ulong value = Sign() ? 0 : ulong.MaxValue;

                    for (int i = buffer.Length - 1; i >= 0; i--)
                    {
                        if (buffer[i] == value)
                        {
                            zero++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (zero > 1)
                    {
                        Array.Resize(ref buffer, buffer.Length - zero + 1);
                    }
                }
            }

            public int Bits()
            {
                bool sign = Sign();
                int bits = 0;

                for (int i = buffer.Length - 1; i >= 0; i--)
                {
                    ulong value = buffer[i];
                    ulong mask = (ulong)1 << (BITS - 1);
                    ulong target = sign ? 0 : mask;

                    for (int j = 0; j < BITS; j++)
                    {
                        if ((value & mask) != target)
                        {
                            return buffer.Length * BITS - bits;
                        }
                        mask >>= 1;
                        target >>= 1;
                        bits++;
                    }
                }
                return 0;
            }

            public bool Sign()
            {
                return buffer[buffer.Length - 1] >> (BITS - 1) == 0;
            }

            public bool Zero()
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != 0) return false;
                }
                return true;
            }

            public override string ToString()
            {
                BigInt temp = Sign() ? this : -this;

                string r = "";
                do
                {
                    r = (temp % 10) + r;
                    temp /= 10;

                } while (!temp.Zero());

                return Sign() ? r : "-" + r;
            }

            public string Sig()
            {
                Shrink();

                ulong result = 0;

                for (int i = 0; i < buffer.Length; i++)
                {
                    result ^= buffer[i];
                }
                return result.ToString("X16");
            }

            public void Alloc(int bits)
            {
                int size = (bits + BITS - 1) / BITS;

                if (buffer.Length < size)
                {
                    Array.Resize(ref buffer, size);
                }
            }

            public static ulong operator &(BigInt p, ulong q)
            {
                return p.buffer[0] & q;
            }

            public static BigInt operator +(BigInt p, BigInt q)
            {
                BigInt r = p.Sign() == q.Sign() ? Add(p, q) : SubNeg(p, q);
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                if (r.ToString() != (_1 + _2).ToString())
                {
                    Console.WriteLine("operator +(BigInt p, BigInt q)");
                }
#endif
                return r;
            }

            public static BigInt operator -(BigInt p, BigInt q)
            {
                BigInt r = p.Sign() == q.Sign() ? Sub(p, q) : AddNeg(p, q);
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                if (r.ToString() != (_1 - _2).ToString())
                {
                    Console.WriteLine("operator -(BigInt p, BigInt q)");
                }
#endif
                r.Shrink();
                return r;
            }

            public static BigInt operator +(BigInt p, ulong carry)
            {
                int n = p.buffer.Length;

                bool sign = p.Sign();

                int size = n;
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _r = r.buffer)
                    {
                        ulong* __p = _p, __r = _r, p_stop = _p + n;
                        ulong temp2;

                        if (sign)
                        {
                            while (__p < p_stop)
                            {
                                temp2 = *__p + carry;

                                *__p = temp2;

                                carry = (ulong)(temp2 < carry ? 1 : 0);

                                __p++; __r++;
                            }
                        }
                        else
                        {
                            while (__p < p_stop)
                            {
                                temp2 = *__p + carry;

                                *__p = temp2;

                                carry = (ulong)(temp2 > carry ? 0 : 1);

                                __p++; __r++;
                            }
                        }
                    }
                }
                if (r.Sign() != sign)
                {
                    Array.Resize(ref r.buffer, r.buffer.Length + 1);
                    r.buffer[r.buffer.Length - 1] = sign ? carry : ulong.MaxValue;
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(carry.ToString());
                if (r.ToString() != (_1 + _2).ToString())
                {
                    Console.WriteLine("operator +(BigInt p, ulong q)");
                }
#endif
                return r;
            }

            public static void AddShift(BigInt p, BigInt q, int bit)
            {
#if TEST
                BigInt r = new BigInt([.. p.buffer]);
#endif
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                int _byte = bit / BITS;
                int _bit = bit % BITS;


                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        ulong* __p = _p + _byte, __q = _q, p_stop = _p + n, q_stop = _q + m;
                        ulong carry = 0, temp1, temp2;

                        if (_bit > 0)
                        {
                            while (__p < p_stop && __q < q_stop)
                            {
                                temp1 = *__p;

                                temp2 = temp1 + (*__q << _bit) + carry;

                                *__p = temp2;

                                carry = (*__q >> (BITS - _bit)) + (ulong)(temp2 < temp1 ? 1 : 0);

                                __p++; __q++;
                            }
                        }
                        else
                        {
                            while (__p < p_stop && __q < q_stop)
                            {
                                temp1 = *__p;

                                temp2 = temp1 + *__q + carry;

                                *__p = temp2;

                                carry = (ulong)(temp2 < temp1 ? 1 : 0);

                                __p++; __q++;
                            }
                        }
                        while (__p < p_stop && carry != 0)
                        {
                            temp2 = *__p + carry;

                            *__p = temp2;

                            carry = (ulong)(temp2 < carry ? 1 : 0);

                            __p++;
                        }
                    }
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                var _3 = BigInteger.Parse(r.ToString());
                if (_1.ToString() != (_3 + (_2 << bit)).ToString())
                {
                    Console.WriteLine("AddShift(BigInt p, BigInt q, int bit)");
                }
#endif
            }

            public static BigInt Add(BigInt p, BigInt q)
            {
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                bool sign = p.Sign();

                int size = Math.Max(n, m);
                BigInt r = new BigInt(new ulong[size]);

                ulong carry = 0;

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __q = _q, __r = _r, p_stop = _p + n, q_stop = _q + m;
                            ulong temp1, temp2;

                            if (sign)
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 + *__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < temp1 ? 1 : 0);

                                    __p++; __q++; __r++;
                                }
                                while (__p < p_stop)
                                {
                                    temp2 = *__p + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? 1 : 0);

                                    __p++; __r++;
                                }
                                while (__q < q_stop)
                                {
                                    temp2 = *__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? 1 : 0);

                                    __q++; __r++;
                                }
                            }
                            else
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 + *__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > temp1 ? 0 : 1);

                                    __p++; __q++; __r++;
                                }
                                carry--;
                                while (__p < p_stop)
                                {
                                    temp2 = *__p + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? ulong.MaxValue : 0);

                                    __p++; __r++;
                                }
                                while (__q < q_stop)
                                {
                                    temp2 = *__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? ulong.MaxValue : 0);

                                    __q++; __r++;
                                }
                            }
                        }
                    }
                }
                if (r.Sign() != sign)
                {
                    Array.Resize(ref r.buffer, r.buffer.Length + 1);
                    r.buffer[r.buffer.Length - 1] = sign ? carry : ulong.MaxValue;
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                if (r.ToString() != (_1 + _2).ToString())
                {
                    Console.WriteLine("Add(BigInt p, BigInt q)");
                }
#endif
                return r;
            }

            public static BigInt AddNeg(BigInt p, BigInt q)
            {
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                bool sign = p.Sign();

                int size = Math.Max(n, m);
                BigInt r = new BigInt(new ulong[size]);

                ulong carry = 1;

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __q = _q, __r = _r, p_stop = _p + n, q_stop = _q + m;
                            ulong temp1, temp2;

                            if (sign)
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 + ~*__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < temp1 ? 1 : 0);

                                    __p++; __q++; __r++;
                                }
                                while (__p < p_stop)
                                {
                                    temp2 = *__p + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? 1 : 0);

                                    __p++; __r++;
                                }
                                while (__q < q_stop)
                                {
                                    temp2 = ~*__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? 1 : 0);

                                    __q++; __r++;
                                }
                            }
                            else
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 + ~*__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > temp1 ? 0 : 1);

                                    __p++; __q++; __r++;
                                }
                                carry--;
                                while (__p < p_stop)
                                {
                                    temp2 = *__p + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? ulong.MaxValue : 0);

                                    __p++; __r++;
                                }
                                while (__q < q_stop)
                                {
                                    temp2 = ~*__q + carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? ulong.MaxValue : 0);

                                    __q++; __r++;
                                }
                            }
                        }
                    }
                }
                if (r.Sign() != sign)
                {
                    Array.Resize(ref r.buffer, r.buffer.Length + 1);
                    r.buffer[r.buffer.Length - 1] = sign ? carry : ulong.MaxValue;
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                if (r.ToString() != (_1 - _2).ToString())
                {
                    Console.WriteLine("AddNeg(BigInt p, BigInt q)");
                }
#endif
                return r;
            }

            public static BigInt Sub(BigInt p, BigInt q)
            {
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                bool sign = p.Sign();

                int size = Math.Max(n, m);
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __q = _q, __r = _r, p_stop = _p + n, q_stop = _q + m;
                            ulong carry = 0, temp1, temp2;

                            if (sign)
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - *__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > temp1 ? 1 : 0);

                                    __p++; __q++; __r++;
                                }
                                while (__p < p_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > temp1 ? 1 : 0);

                                    __p++; __r++;
                                }
                                carry--;
                                while (__q < q_stop)
                                {
                                    temp2 = ~*__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > 0 ? 0 : ulong.MaxValue);
                                    
                                    __q++; __r++;
                                }
                            }
                            else
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - *__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < temp1 ? 0 : 1);

                                    __p++; __q++; __r++;
                                }
                                carry--;
                                while (__p < p_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < temp1 ? ulong.MaxValue : 0);

                                    __p++; __r++;
                                }
                                carry++;
                                while (__q < q_stop)
                                {
                                    temp2 = ~*__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < ulong.MaxValue ? 0 : 1);

                                    __q++; __r++;
                                }
                            }
                        }
                    }
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                if (r.ToString() != (_1 - _2).ToString())
                {
                    Console.WriteLine("Sub(BigInt p, BigInt q)");
                }
#endif
                return r;
            }

            public static BigInt SubNeg(BigInt p, BigInt q)
            {
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                bool sign = p.Sign();

                int size = Math.Max(n, m);
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __q = _q, __r = _r, p_stop = _p + n, q_stop = _q + m;
                            ulong carry = 1, temp1, temp2;

                            if (sign)
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - ~*__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > temp1 ? 1 : 0);

                                    __p++; __q++; __r++;
                                }
                                while (__p < p_stop)
                                {
                                    temp2 = *__p - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < carry ? 1 : 0);

                                    __p++; __r++;
                                }
                                carry--;
                                while (__q < q_stop)
                                {
                                    temp2 = *__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 > 0 ? 0 : ulong.MaxValue);

                                    __q++; __r++;
                                }
                            }
                            else
                            {
                                while (__p < p_stop && __q < q_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - ~*__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < temp1 ? 0 : 1);

                                    __p++; __q++; __r++;
                                }
                                carry--;
                                while (__p < p_stop)
                                {
                                    temp1 = *__p;

                                    temp2 = temp1 - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < temp1 ? ulong.MaxValue : 0);

                                    __p++; __r++;
                                }
                                carry++;
                                while (__q < q_stop)
                                {
                                    temp2 = *__q - carry;

                                    *__r = temp2;

                                    carry = (ulong)(temp2 < ulong.MaxValue ? 0 : 1);

                                    __q++; __r++;
                                }
                            }
                        }
                    }
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                var _2 = BigInteger.Parse(q.ToString());
                if (r.ToString() != (_1 + _2).ToString())
                {
                    Console.WriteLine("SubNeg(BigInt p, BigInt q)");
                }
#endif
                return r;
            }

            public static BigInt operator -(BigInt p)
            {
                int n = p.buffer.Length;

                int size = n;
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _r = r.buffer)
                    {
                        ulong* __p = _p, __r = _r, p_stop = _p + n;
                        ulong carry = 1, temp2;

                        while (__p < p_stop)
                        {
                            temp2 = ~*__p + carry;

                            *__r = temp2;

                            carry = (ulong)(temp2 < carry ? 1 : 0);

                            __p++; __r++;
                        }
                    }
                }
                return r;
            }

            public static BigInt operator <<(BigInt p, int bit)
            {
                int n = p.buffer.Length;
                int _byte = bit / BITS;
                int _bit = bit % BITS;

                BigInt r;

                if (_bit == 0)
                {
                    r = new BigInt(new ulong[n + _byte]);

                    Array.Copy(p.buffer, 0, r.buffer, _byte, n);
                }
                else
                {
                    r = new BigInt(new ulong[n + _byte + 1]);

                    fixed (ulong* _p = p.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __r = _r + _byte, p_stop = _p + n;
                            ulong carry = 0, temp1;

                            while (__p < p_stop)
                            {
                                temp1 = *__p;

                                * __r = (temp1 << _bit) | carry;

                                carry = temp1 >> (BITS - _bit);

                                __p++; __r++;
                            }
                            *__r = ((p.Sign() ? 0 : ulong.MaxValue) << _bit) | carry;
                        }
                    }
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                if (r.ToString() != (_1 << bit).ToString())
                {
                    Console.WriteLine("operator <<(BigInt p, int bit)");
                }
#endif
                return r;
            }

            public static BigInt operator >>(BigInt p, int bit)
            {
                int n = p.buffer.Length;
                int _byte = bit / BITS;
                int _bit = bit % BITS;

                BigInt r;

                if (n > _byte)
                {
                    r = new BigInt(new ulong[n - _byte]);

                    if (_bit == 0)
                    {
                        Array.Copy(p.buffer, _byte, r.buffer, 0, n - _byte);
                    }
                    else
                    {
                        fixed (ulong* _p = p.buffer)
                        {
                            fixed (ulong* _r = r.buffer)
                            {
                                ulong* __p = _p + n - 1, __r = _r + n - _byte - 1, p_stop = _p + _byte;
                                ulong carry = (p.Sign() ? 0 : ulong.MaxValue) << (BITS - _bit), temp1;

                                while (__p >= p_stop)
                                {
                                    temp1 = *__p;

                                    *__r = (temp1 >> _bit) | carry;

                                    carry = temp1 << (BITS - _bit);

                                    __p--; __r--;
                                }
                            }
                        }
                    }
                }
                else
                {
                    r = 0;
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                if (p.Sign() && r.ToString() != (_1 >> bit).ToString())
                {
                    Console.WriteLine("operator >>(BigInt p, int bit)");
                }
#endif
                r.Shrink();
                return r;
            }

            public static BigInt operator >>>(BigInt p, int bit)
            {
                int n = p.buffer.Length;

                int _byte = bit / BITS;
                int _bit = bit % BITS;

                BigInt r;

                if (n > _byte)
                {
                    r = new BigInt(new ulong[n - _byte]);

                    if (_bit == 0)
                    {
                        Array.Copy(p.buffer, _byte, r.buffer, 0, n - _byte);
                    }
                    else
                    {
                        fixed (ulong* _p = p.buffer)
                        {
                            fixed (ulong* _r = r.buffer)
                            {
                                ulong* __p = _p + n - 1, __r = _r + n - _byte - 1, p_stop = _p + _byte;
                                ulong carry = 0, temp1;

                                while (__p >= p_stop)
                                {
                                    temp1 = *__p;

                                    *__r = (temp1 >> _bit) | carry;

                                    carry = temp1 << (BITS - _bit);

                                    __p--; __r--;
                                }
                            }
                        }
                    }
                }
                else
                {
                    r = 0;
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                if (r.ToString() != (_1 >>> bit).ToString())
                {
                    Console.WriteLine("operator >>>(BigInt p, int bit)");
                }
#endif
                r.Shrink();
                return r;
            }

            public static BigInt operator &(BigInt p, BigInt q)
            {
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                int size = Math.Min(n, m);
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __q = _q, __r = _r, p_stop = _p + n, q_stop = _q + m;

                            while (__p < p_stop && __q < q_stop)
                            {
                                *__r++ = *__p++ & *__q++;
                            }
                        }
                    }
                }
                if (!r.Sign())
                {
                    Array.Resize(ref r.buffer, r.buffer.Length + 1);
                }
                r.Shrink();
                return r;
            }

            public static BigInt operator |(BigInt p, BigInt q)
            {
                int n = p.buffer.Length;
                int m = q.buffer.Length;

                int size = Math.Max(n, m);
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _q = q.buffer)
                    {
                        fixed (ulong* _r = r.buffer)
                        {
                            ulong* __p = _p, __q = _q, __r = _r, p_stop = _p + n, q_stop = _q + m;

                            while (__p < p_stop && __q < q_stop)
                            {
                                *__r = *__p | *__q;

                                __p++; __q++; __r++;
                            }
                            while (__p < p_stop)
                            {
                                *__r = *__p;

                                __p++; __r++;
                            }
                            while (__q < q_stop)
                            {
                                *__r = *__q;

                                __q++; __r++;
                            }
                        }
                    }
                }
                if (!r.Sign())
                {
                    Array.Resize(ref r.buffer, r.buffer.Length + 1);
                }
                return r;
            }

            public static BigInt operator ~(BigInt p)
            {
                int n = p.buffer.Length;

                int size = n;
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _r = r.buffer)
                    {
                        ulong* __p = _p, __r = _r, p_stop = _p + n;

                        while (__p < p_stop)
                        {
                            *__r++ = ~*__p++;
                        }
                    }
                }
#if TEST
                var _1 = BigInteger.Parse(p.ToString());
                if (r.ToString() != (~_1).ToString())
                {
                    Console.WriteLine("operator &(BigInt p, BigInt q)");
                }
#endif
                return r;
            }

            public static BigInt operator /(BigInt p, ulong q)
            {
                int n = p.buffer.Length;

                bool sign = p.Sign() == (q > 0);

                p = sign ? p : -p;

                int size = n;
                BigInt r = new BigInt(new ulong[size]);

                fixed (ulong* _p = p.buffer)
                {
                    fixed (ulong* _r = r.buffer)
                    {
                        uint* __p = (uint*)(_p + n - 1) + 1, __r = (uint*)(_r + n - 1) + 1, p_stop = (uint*)_p;
                        ulong carry = 0;

                        while (__p >= p_stop)
                        {
                            var result = Math.DivRem(*__p + (carry << BITS_HALF), q);

                            *__r = (uint)result.Quotient;

                            carry = result.Remainder;

                            __p--; __r--;
                        }

                    }
                }

                r = sign ? r : -r;

                r.Shrink();
                return r;
            }

            public static ulong operator %(BigInt p, ulong q)
            {
                int n = p.buffer.Length;

                p = p.Sign() ? p : -p;

                ulong r = 0;

                fixed (ulong* _p = p.buffer)
                {
                    uint* __p = (uint*)(_p + n - 1) + 1, p_stop = (uint*)_p;

                    while (__p >= p_stop)
                    {
                        r = (*__p + (r << BITS_HALF)) % q;

                        __p--;
                    }
                }
                return r;
            }

            public static BigInt operator *(BigInt p, BigInt q)
            {
                bool small = (p.buffer.Length + q.buffer.Length) * BITS < SIZE_CUTOFF;

                if (p == q)
                {
                    return small ? Multiply(p) : ToomCook3(p, true);
                }
                else
                {
                    return small ? Multiply(p, q) : ToomCook3(p, q, true);
                }
            }
        }
    }
}
