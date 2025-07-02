//#define TEST

using System.Diagnostics;
using System.Numerics;

namespace Fibonacci
{
    partial class Program
    {
        public delegate BigInt Execute(ulong n);

        const int BEGIN = 2;
        const int REPEAT = 8;
        const int END_ADD = 1024 * 1024 * 16;
        const int END_MUL = 1024 * 1024;
        const int END_FIB = 1024 * 1024 * 4;

        static Random rand = new Random(DateTime.Now.Second);

        static void Add(string sign, ulong n, BigInt a1, BigInt a2, BigInteger c1, BigInteger c2)
        {
            Stopwatch timer = new();
            long time1, time2;
            BigInt r1 = a1;
            BigInteger r2;

            timer.Start();
            for (int i = 0; i < REPEAT; i++) r2 = c1 + c2;
            timer.Stop();
            time1 = timer.ElapsedMilliseconds;

            timer.Reset();
            timer.Start();
            for (int i = 0; i < REPEAT; i++) r1 = a1 + a2;
            timer.Stop();
            time2 = timer.ElapsedMilliseconds;

            if (time1 > 0 && time2 >0 && time1 != time2) Console.WriteLine("byte[{0}] BigInt a + b ({1}) {2} mS => {3} mS ({4} x faster)", n * BYTES, sign, time1, time2, Math.Round(time1 / (double)time2, 1));
        }

        static void Sub(string sign, ulong n, BigInt a1, BigInt a2, BigInteger c1, BigInteger c2)
        {
            Stopwatch timer = new();
            long time1, time2;
            BigInt r1 = a1;
            BigInteger r2;

            timer.Start();
            for (int i = 0; i < REPEAT; i++) r2 = c1 - c2;
            timer.Stop();
            time1 = timer.ElapsedMilliseconds;

            timer.Reset();
            timer.Start();
            for (int i = 0; i < REPEAT; i++) r1 = a1 - a2;
            timer.Stop();
            time2 = timer.ElapsedMilliseconds;

            if (time1 > 0 && time2 >0 && time1 != time2) Console.WriteLine("byte[{0}] BigInt a - b ({1}) {2} mS => {3} mS ({4} x faster)", n * BYTES, sign, time1, time2, Math.Round(time1 / (double)time2, 1));
        }

        static void Mod(string sign, ulong n, BigInt a1, BigInteger c1, ulong d)
        {
            Stopwatch timer = new();
            long time1, time2;
            BigInt r1;
            BigInteger r2;

            timer.Start();
            for (int i = 0; i < REPEAT; i++) r2 = c1 % d;
            timer.Stop();
            time1 = timer.ElapsedMilliseconds;

            timer.Reset();
            timer.Start();
            for (int i = 0; i < REPEAT; i++) r1 = a1 % d;
            timer.Stop();
            time2 = timer.ElapsedMilliseconds;

            if (time1 > 0 && time2 >0 && time1 != time2) Console.WriteLine("byte[{0}] BigInt a % b ({1}) {2} mS => {3} mS ({4} x faster)", n * BYTES, sign, time1, time2, Math.Round(time1 / (double)time2, 1));
        }

        static void Div(string sign, ulong n, BigInt a1, BigInteger c1, ulong d)
        {
            Stopwatch timer = new();
            long time1, time2;
            BigInt r1;
            BigInteger r2;

            timer.Start();
            for (int i = 0; i < REPEAT; i++) r2 = c1 / d;
            timer.Stop();
            time1 = timer.ElapsedMilliseconds;

            timer.Reset();
            timer.Start();
            for (int i = 0; i < REPEAT; i++) r1 = a1 / d;
            timer.Stop();
            time2 = timer.ElapsedMilliseconds;

            if (time1 > 0 && time2 >0 && time1 != time2) Console.WriteLine("byte[{0}] BigInt a / b ({1}) {2} mS => {3} mS ({4} x faster)", n * BYTES, sign, time1, time2, Math.Round(time1 / (double)time2, 1));
        }

        static void Addition(ulong n = 100000)
        {
            ulong[] b1 = new ulong[n], b2 = new ulong[n];

            for (int i = 0; i < b1.Length; i++)
            {
                b1[i] = (ulong)rand.NextInt64();
                b2[i] = (ulong)rand.NextInt64();
            }
            switch(rand.Next() % 10)
            {
                case 0:
                    if (n > 1) Array.Resize(ref b1, b1.Length / 2); break;
                case 1:
                    if (n > 1) Array.Resize(ref b2, b2.Length / 2); break;
            }

            byte[] b3 = new byte[b1.Length * BYTES], b4 = new byte[b2.Length * BYTES];

            Buffer.BlockCopy(b1, 0, b3, 0, b3.Length);
            Buffer.BlockCopy(b2, 0, b4, 0, b4.Length);

            BigInt a1 = new BigInt(b1), a2 = new BigInt(b2);
            BigInteger c1 = new BigInteger(b3), c2 = new BigInteger(b4);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////

            var r1 = a1 + a2;
            var r2 = c1 + c2;

            Add("+ve +ve", n, a1, a2, c1, c2);
            a1 = -a1;
            c1 = -c1;
            Add("-ve +ve", n, a1, a2, c1, c2);
            a1 = -a1;
            a2 = -a2;
            c1 = -c1;
            c2 = -c2;
            Add("+ve -ve", n, a1, a2, c1, c2);
            a1 = -a1;
            c1 = -c1;
            Add("-ve -ve", n, a1, a2, c1, c2);
            a1 = -a1;
            a2 = -a2;
            c1 = -c1;
            c2 = -c2;

            Sub("+ve +ve", n, a1, a2, c1, c2);
            a1 = -a1;
            c1 = -c1;
            Sub("-ve +ve", n, a1, a2, c1, c2);
            a1 = -a1;
            a2 = -a2;
            c1 = -c1;
            c2 = -c2;
            Sub("+ve -ve", n, a1, a2, c1, c2);
            a1 = -a1;
            c1 = -c1;
            Sub("-ve -ve", n, a1, a2, c1, c2);
            a1 = -a1;
            a2 = -a2;
            c1 = -c1;
            c2 = -c2;

            Mod("+ve +ve", n, a1, c1, 456376323);
            a1 = -a1;
            c1 = -c1;
            Mod("-ve +ve", n, a1, c1, 456376323);
            a1 = -a1;
            c1 = -c1;

            Div("+ve +ve", n, a1, c1, 456376323);
            a1 = -a1;
            c1 = -c1;
            Div("-ve +ve", n, a1, c1, 456376323);
        }

        static void Multiplication(ulong n = 100000)
        {
            Stopwatch timer = new();
            double time1, time2;

            ulong[] b1 = new ulong[n], b2 = new ulong[n];

            for (int i = 0; i < b1.Length; i++)
            {
                b1[i] = (ulong)rand.NextInt64();
                b2[i] = (ulong)rand.NextInt64();
            }

            switch (rand.Next() % 10)
            {
                case 0:
                    if (n > 1) Array.Resize(ref b1, b1.Length / 2); break;
                case 1:
                    if (n > 1) Array.Resize(ref b2, b2.Length / 2); break;
            }

            byte[] b3 = new byte[b1.Length * BYTES], b4 = new byte[b2.Length * BYTES];

            Buffer.BlockCopy(b1, 0, b3, 0, b3.Length);
            Buffer.BlockCopy(b2, 0, b4, 0, b4.Length);

            BigInt a1 = new BigInt(b1), a2 = new BigInt(b2), r1;
            BigInteger c1 = new BigInteger(b3), c2 = new BigInteger(b4), r2;

            string sign = "";
            sign += a1.Sign() ? "+ve " : "-ve ";
            sign += a2.Sign() ? "+ve" : "-ve";

            timer.Start();
            r2 = c1 * c1;
            timer.Stop();
            time1 = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            r1 = a1 * a1;
            timer.Stop();
            time2 = timer.ElapsedMilliseconds;
            if (time1 > 0 && time2 >0 && time1 != time2) Console.WriteLine("byte[{0}] BigInt a ^ 2 ({1}) {2} mS => {3} mS ({4} x faster)", n * BYTES, sign, time1, time2, Math.Round(time1 / (double)time2, 1));

            timer.Reset();
            timer.Start();
            r2 = c1 * c2;
            timer.Stop();
            time1 = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
            r1 = a1 * a2;
            timer.Stop();
            time2 = timer.ElapsedMilliseconds;
            if (time1 > 0 && time2 >0 && time1 != time2) Console.WriteLine("byte[{0}] BigInt a * b ({1}) {2} mS => {3} mS ({4} x faster)", n * BYTES, sign, time1, time2, Math.Round(time1 / (double)time2, 1));
        }

        static void Once(string name, Execute fibonacci, ulong n = 100000)
        {
            cache.Clear();
            cache_BigInteger.Clear();
            DateTime start = DateTime.Now;
            BigInt result = fibonacci(n);
            DateTime stop = DateTime.Now;

            string signature = result.Sig();

            Console.WriteLine("{0} F({1:n0}) = [{2}] / {3}", name, n, signature.Replace("-", ""), stop.Subtract(start).TotalSeconds);
        }

        static void Second(string name, Execute fibonacci, ulong n = 300000)
        {
            TimeSpan diff;

            do
            {
                n = n * 101 / 100;

                Console.WriteLine("\t" + n);

                cache.Clear();
                cache_BigInteger.Clear();
                DateTime start = DateTime.Now;
                BigInt result = fibonacci(n);
                DateTime stop = DateTime.Now;

                diff = stop.Subtract(start);

            } while (diff.TotalSeconds < 1.0);

            do
            {
                n = n * 99 / 100;

                Console.WriteLine("\t" + n);

                cache.Clear();
                cache_BigInteger.Clear();
                DateTime start = DateTime.Now;
                BigInt result = fibonacci(n);
                DateTime stop = DateTime.Now;

                diff = stop.Subtract(start);

            } while (diff.TotalSeconds > 1.0);

            BigInt _signature = fibonacci(1000);
            string signature = _signature.Sig();

            Console.WriteLine("{0} F(100,000) = [{1}...] {2:n0} / sec {3}", name, signature, n, n);
        }

        static void Main(string[] args)
        {
#if TEST
            {
                BigInt result = FibonacciMultiply(1000);
                string _result = result.ToString();
                if (_result != "43466557686937456435688527675040625802564660517371780402481729089536555417949051890403879840079255169295922593080322634775209689623239873322471161642996440906533187938298969649928516003704476137795166849228875")
                {
                    Console.WriteLine(_result);
                }
            }
            {
                BigInt result = FibonacciMultiply(10000);
                string _result = result.ToString();
                if (_result != "33644764876431783266621612005107543310302148460680063906564769974680081442166662368155595513633734025582065332680836159373734790483865268263040892463056431887354544369559827491606602099884183933864652731300088830269235673613135117579297437854413752130520504347701602264758318906527890855154366159582987279682987510631200575428783453215515103870818298969791613127856265033195487140214287532698187962046936097879900350962302291026368131493195275630227837628441540360584402572114334961180023091208287046088923962328835461505776583271252546093591128203925285393434620904245248929403901706233888991085841065183173360437470737908552631764325733993712871937587746897479926305837065742830161637408969178426378624212835258112820516370298089332099905707920064367426202389783111470054074998459250360633560933883831923386783056136435351892133279732908133732642652633989763922723407882928177953580570993691049175470808931841056146322338217465637321248226383092103297701648054726243842374862411453093812206564914032751086643394517512161526545361333111314042436854805106765843493523836959653428071768775328348234345557366719731392746273629108210679280784718035329131176778924659089938635459327894523777674406192240337638674004021330343297496902028328145933418826817683893072003634795623117103101291953169794607632737589253530772552375943788434504067715555779056450443016640119462580972216729758615026968443146952034614932291105970676243268515992834709891284706740862008587135016260312071903172086094081298321581077282076353186624611278245537208532365305775956430072517744315051539600905168603220349163222640885248852433158051534849622434848299380905070483482449327453732624567755879089187190803662058009594743150052402532709746995318770724376825907419939632265984147498193609285223945039707165443156421328157688908058783183404917434556270520223564846495196112460268313970975069382648706613264507665074611512677522748621598642530711298441182622661057163515069260029861704945425047491378115154139941550671256271197133252763631939606902895650288268608362241082050562430701794976171121233066073310059947366875")
                {
                    Console.WriteLine(_result);
                }
            }
#endif
            ulong n;

            n = BEGIN;
            while (n <= END_ADD)
            {
                Addition(n);

                n = n * 2;
            }

            n = BEGIN;
            while (n <= END_MUL)
            {
                Multiplication(n);

                n = n * 2;
            }

            n = BEGIN;
            while (n <= END_FIB)
            {
                Once("FibonacciDivideThreadCache        ", FibonacciDivideThreadCache, n);
                Once("FibonacciMultiplyToomCook3        ", FibonacciMultiplyToomCook3, n);
                Once("FibonacciDivideAsyncCache         ", FibonacciDivideAsyncCache, n);
                Once("FibonacciDivideCache              ", FibonacciDivideCache, n);
                Once("FibonacciMultiplyBigInteger       ", FibonacciMultiplyBigInteger, n);
                Once("FibonacciDivideThread             ", FibonacciDivideThread, n);
                Once("FibonacciMultiplyToomCook3Tuple   ", FibonacciMultiplyToomCook3Tuple, n);
                Once("FibonacciDivide                   ", FibonacciDivide, n);

                Once("FibonacciMultiplyKaratsuba        ", FibonacciMultiplyKaratsuba, n);
                Once("FibonacciMultiplySchönhageStrassen", FibonacciMultiplySchönhageStrassen, n);
                Once("FibonacciMultiply                 ", FibonacciMultiply, n);

                n = n * 2;
            }

            Second("FibonacciDivideThreadCache        ", FibonacciDivideThreadCache, 17938524);
            Second("FibonacciDivideThreadCache        ", FibonacciDivideThreadCache, 17938524);
            Second("FibonacciDivideAsyncCache         ", FibonacciDivideAsyncCache, 16072292);
            Second("FibonacciMultiplyToomCook3        ", FibonacciMultiplyToomCook3, 15991172);
            Second("FibonacciDivideCache              ", FibonacciDivideCache, 15768224);
            Second("FibonacciMultiplyBigInteger       ", FibonacciMultiplyBigInteger, 14175112);
            Second("FibonacciDivideThread             ", FibonacciDivideThread, 11838297);
            Second("FibonacciMultiplyToomCook3Tuple   ", FibonacciMultiplyToomCook3Tuple, 12668553);
            Second("FibonacciDivide                   ", FibonacciDivide, 8968069);
            Second("FibonacciMultiplyKaratsuba        ", FibonacciMultiplyKaratsuba, 6825016);
            Second("FibonacciMultiplySchönhageStrassen", FibonacciMultiplySchönhageStrassen, 2379174);
            Second("FibonacciMultiply                 ", FibonacciMultiply, 829917);
            Second("FibonacciNaiveOptimised           ", FibonacciNaiveOptimised, 442670);
            Second("FibonacciNaive                    ", FibonacciNaive, 447469);
            Second("FibonacciNaiveVector              ", FibonacciNaiveVector, 391989);
        }
    }
}
