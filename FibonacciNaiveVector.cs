using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Runtime.InteropServices;

namespace Fibonacci
{
    partial class Program
    {
        unsafe static BigInt FibonacciNaiveVector(ulong n)
        {
            ulong size = Size(n);

            ulong[] _a = new ulong[size], _b = new ulong[size];

            ulong* a = (ulong*)NativeMemory.AlignedAlloc(sizeof(ulong) * (uint)size, sizeof(ulong) * 4);
            ulong* b = (ulong*)NativeMemory.AlignedAlloc(sizeof(ulong) * (uint)size, sizeof(ulong) * 4);
            ulong** d = (ulong**)NativeMemory.Alloc((uint)sizeof(ulong*) * 2);

            for (ulong m = 0; m < size; m++)
            {
                a[m] = b[m] = 0;
            }

            ulong _msb = (ulong)1 << (BITS - 1);
            Vector256<ulong> shift = Vector256.Create((ulong)BITS - 1, BITS - 1, BITS - 1, BITS - 1);
            Vector256<ulong> msb = Vector256.Create(_msb, _msb, _msb, _msb);
            Vector256<ulong> first = Vector256.Create(Vector256<ulong>.AllBitsSet.GetElement(0), 0, 0, 0);
            Vector256<ulong> not_first = Avx2.Xor(first, Vector256<ulong>.AllBitsSet);

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

                ulong* avx = r + j / 4;

                ulong c = 0, t;
                Vector256<ulong> _c = Vector256<ulong>.Zero, __c, _t, tmp;

                while (r < avx)
                {
                    var _r = Avx2.LoadAlignedVector256(r);
                    var _w = Avx2.LoadAlignedVector256(w);

                    //add
                    _t = Avx2.Add(_r, _w);
                    _t = Avx2.Add(_t, _c);

                    //calculate carry (There is no ulong support)
                    tmp = Avx2.CompareGreaterThan(_r.AsInt64(), _t.AsInt64()).AsUInt64();
                    tmp = Avx2.Xor(tmp, _t);
                    tmp = Avx2.Xor(tmp, _r);
                    tmp = Avx2.And(tmp, msb);
                    _c = Avx2.ShiftRightLogical(tmp, BITS - 1);

                    //save carry
                    __c = _c;

                    while (_c.GetElement(0) == 1 || _c.GetElement(1) == 1 || _c.GetElement(2) == 1)
                    {
                        //right shift elements, zero first
                        _c = Avx2.Permute4x64(_c, 0x00 << 0 | 0x00 << 2 | 0x01 << 4 | 0x02 << 6);
                        _c = Avx2.And(_c, not_first);

                        //add carry
                        _t = Avx2.Add(_t, _c);

                        //calculate carry
                        tmp = Avx2.CompareEqual(_t, Vector256<ulong>.AllBitsSet);
                        _c = Avx2.ShiftRightLogical(tmp, BITS - 1);
                    }

                    //combine carry and move to first
                    _c = Avx2.Or(_c, __c);
                    _c = Avx2.Permute4x64(_c, 0x03 << 0 | 0x00 << 2 | 0x00 << 4 | 0x00 << 6);
                    _c = Avx2.And(_c, first);

                    Avx2.StoreAligned(w, _t);

                    r += 4; w += 4;
                }
                c = _c.GetElement(0);

                //rest
                while (r < s)
                {
                    t = *r + *w + c;
                    c = (ulong)(*w > *r + *w + c ? 1 : 0);
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
            for (ulong m = 0; m < size; m++)
            {
                _a[m] = a[m];
                _b[m] = b[m];
            }
            NativeMemory.AlignedFree(a);
            NativeMemory.AlignedFree(b);
            NativeMemory.Free(d);

            return new BigInt((n & 1) == 0 ? _a : _b);
        }
    }
}
