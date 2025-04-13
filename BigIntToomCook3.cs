namespace Fibonacci
{
    partial class Program
    {
        static BigInt ToomCook3(BigInt p, bool top = false)
        {
            int n = p.Bits();

            if (n < SIZE_CUTOFF)
            {
                return Multiply(p);
            }

            n = (n + 2) / 3;

            bool sign = true;
            p = p.Sign() ? p : -p;

            BigInt[] _p = Extract(n, p);

            if (THREAD && top)
            {
                Task[] task =
                [
                    Task.Run(() => { _p[0] = ToomCook3(_p[0]); }),
                    Task.Run(() => { _p[1] = ToomCook3(_p[1]); }),
                    Task.Run(() => { _p[2] = ToomCook3(_p[2]); }),
                    Task.Run(() => { _p[3] = ToomCook3(_p[3]); }),
                    Task.Run(() => { _p[4] = ToomCook3(_p[4]); }),
                ];
                Task.WaitAll(task);
            }
            else
            {
                _p[0] = ToomCook3(_p[0]);
                _p[1] = ToomCook3(_p[1]);
                _p[2] = ToomCook3(_p[2]);
                _p[3] = ToomCook3(_p[3]);
                _p[4] = ToomCook3(_p[4]);
            }

            BigInt[] _r = new BigInt[5];
            _r[0] = _p[0];
            _r[4] = _p[4];
            _r[3] = (_p[3] - _p[1]) / 3;
            _r[1] = (_p[1] - _p[2]) >> 1;
            _r[2] = _p[2] - _p[0];
            _r[3] = ((_r[2] - _r[3]) >> 1) + (_p[4] << 1);
            _r[2] = _r[2] + _r[1] - _r[4];
            _r[1] = _r[1] - _r[3];

            var __r = new BigInt();

            __r.Alloc(4 * n + _r[4].buffer.Length * BITS);

            for (int i = 0; i < _r.Length; i++)
            {
                BigInt.AddShift(__r, _r[i], n * i);
            }

            __r = sign ? __r : -__r;

            return __r;
        }

        static BigInt ToomCook3(BigInt p, BigInt q, bool top = false)
        {
            int n = Math.Max(p.Bits(), q.Bits());

            if (n < SIZE_CUTOFF)
            {
                return Multiply(p, q);
            }

            n = (n + 2) / 3;

            bool sign = p.Sign() == q.Sign();
            p = p.Sign() ? p : -p;
            q = q.Sign() ? q : -q;

            BigInt[] _p = Extract(n, p), _q = Extract(n, q);

            if (THREAD && top)
            {
                Task[] task =
                [
                    Task.Run(() => { _p[0] = ToomCook3(_p[0], _q[0]); }),
                    Task.Run(() => { _p[1] = ToomCook3(_p[1], _q[1]); }),
                    Task.Run(() => { _p[2] = ToomCook3(_p[2], _q[2]); }),
                    Task.Run(() => { _p[3] = ToomCook3(_p[3], _q[3]); }),
                    Task.Run(() => { _p[4] = ToomCook3(_p[4], _q[4]); }),
                ];
                Task.WaitAll(task);
            }
            else
            {
                _p[0] = ToomCook3(_p[0], _q[0]);
                _p[1] = ToomCook3(_p[1], _q[1]);
                _p[2] = ToomCook3(_p[2], _q[2]);
                _p[3] = ToomCook3(_p[3], _q[3]);
                _p[4] = ToomCook3(_p[4], _q[4]);
            }

            BigInt[] _r = new BigInt[5];
            _r[0] = _p[0];
            _r[4] = _p[4];
            _r[3] = (_p[3] - _p[1]) / 3;
            _r[1] = (_p[1] - _p[2]) >> 1;
            _r[2] = _p[2] - _p[0];
            _r[3] = ((_r[2] - _r[3]) >> 1) + (_p[4] << 1);
            _r[2] = _r[2] + _r[1] - _r[4];
            _r[1] = _r[1] - _r[3];

            var __r = new BigInt();

            __r.Alloc(4 * n + _r[4].buffer.Length * BITS);

            for (int i = 0; i < _r.Length; i++)
            {
                BigInt.AddShift(__r, _r[i], n * i);
            }

            __r = sign ? __r : -__r;

            return __r;
        }

        static BigInt[] Extract(int n, BigInt p)
        {
            var m = new BigInt[3];

            BigInt mask = ((BigInt)1 << n) - 1;

            for (int i = 0; i < m.Length; i++)
            {
                m[i] = p & mask;
                p >>= n;
            }

            var _p = new BigInt[5];

            var p0 = m[0] + m[2];

            _p[0] = m[0];
            _p[1] = p0 + m[1];
            _p[2] = p0 - m[1];
            _p[3] = ((_p[2] + m[2]) << 1) - m[0];
            _p[4] = m[2];

            return _p;
        }
    }
}
