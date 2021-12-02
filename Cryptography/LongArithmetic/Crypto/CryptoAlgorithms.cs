using System;
using System.Collections.Generic;
using LongArithmetic;

namespace Crypto
{
    public class CryptoAlgorithms
    {
        private const int ALPH_BASE = 28;
        private const int ALPH_DIFF = 64;
        private static int[] primes = { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 257 };

        public static LongNum GenerateNBitPrime(int n, int seed=-1) {
            LongNum num = 0;
            while (num > 257 && DivisibleBySmallPrime(num) || !IsPrimeMillerRabin(num)) {                
                num = LongNum.Rand(LongNum.Pow(2, n) - 1, LongNum.Pow(2, n + 1), seed);
            }
            return num;
        }

        public static LongNum RandPrime(LongNum a, LongNum b, int seed=-1) {
            LongNum res = 0;
            while (!IsPrimeMillerRabin(res)) {
                res = LongNum.Rand(a, b, seed);
            }

            return res;
        }

        // Факторизація Полларда
        public static (LongNum, LongNum) FactorizePollard(LongNum n) {
            var rnd = new Random();
            LongNum x = n < int.MaxValue ? rnd.Next(0, n) : rnd.Next(0, int.MaxValue);
            LongNum y = x;
            LongNum d = new LongNum(1);

            while (d == 1) {
                x = F(x, n);
                y = F(F(y, n), n);
                d = LongNum.Gcd(n, LongNum.Abs(x - y));
            }

            return d == n ? (null, null) : (d, n / d);
        }
        
        private static bool DivisibleBySmallPrime(LongNum n) {
            foreach (var p in primes) {
                if (n % p == 0) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPrimeMillerRabin(LongNum n, int fixedRand=0) {
            if (n == 2 || n == 3)
                return true;
            if (n < 2 || n % 2 == 0)
                return false;

            int k = 10;
            var d = n - 1;
            LongNum r = 0;
            while (d % 2 == 0) {
                d /= 2;
                r++;
            }


            for (int i = 0; i < k; i++) {
                var a = fixedRand == 0 ? LongNum.Rand(2, n - 1) : new LongNum(fixedRand);
                var x = LongNum.PowMod(a, d, n);

                if (x != 1 && x != n - 1 && !ContinueMillerRabin(r, x, n)) {
                    return false;
                }
            }
            return true;
        }

        private static bool ContinueMillerRabin(LongNum r, LongNum x, LongNum n) {
            for (int j = 0; j < r - 1; j++) {
                x = LongNum.PowMod(x, 2, n);

                if (x == n - 1) {
                    return true;
                }
            }
            return false;
        }

        public static bool IsPrimeBailliePsw(LongNum n) {
            if (n == 2)
                return true;
            if (n < 2 || n % 2 == 0 || n.IsPerfectSquare())
                return false;

            var D = Selfridge(n);
            if (D is null) {
                return false;
            }

            LongNum P = 1;
            var Q = (1 - D) / 4;

            var d = n - 1;
            LongNum r = 0;
            while (D % 2 == 0) {
                D /= 2;
                r++;
            }

            return IsPrimeLucas(P, Q, D, d, n, r);
        }

        private static bool IsPrimeLucas(LongNum P, LongNum Q, LongNum D, LongNum d, LongNum n, LongNum r) {
            LongNum u = 1, v = P, u2m = 1, v2m = P, qm = Q, qm2 = Q * 2, qkd = Q;

            for (int b = 1, bits = d.Bitness(); b < bits; b++) {
                u2m = LongNum.MulMod(u2m, v2m, n);
                v2m = LongNum.MulMod(v2m, v2m, n);
                while (v2m < qm2)
                    v2m += n;
                v2m -= qm2;
                qm = LongNum.PowMod(qm, 2, n);
                qm2 = qm * 2;
                if (d.BitAt(b) != 0) {
                    LongNum t1, t2;
                    t1 = u2m;
                    t1 = LongNum.MulMod(t1, v, n);
                    t2 = v2m;
                    t2 = LongNum.MulMod(t2, u, n);

                    LongNum t3, t4;
                    t3 = v2m;
                    t3 = LongNum.MulMod(t3, v, n);
                    t4 = u2m;
                    t4 = LongNum.MulMod(t4, u, n);
                    t4 = LongNum.MulMod(t4, D, n);

                    u = t1 + t2;
                    if (u % 2 != 0)
                        u += n;
                    u /= 2;
                    u %= n;

                    v = t3 + t4;
                    if (v % 2 != 0)
                        v += n;
                    v /= 2;
                    qkd = LongNum.MulMod(qkd, qm, n);
                }
            }

            if (u == 0 || v == 0)
                return true;

            var qkd2 = qkd * 2;
            for (LongNum x = 1; x < r; x++) {
                v = LongNum.PowMod(v, 2, n);
                v -= qkd2;
                if (v < 0) v += n;
                if (v < 0) v += n;
                if (v >= n) v -= n;
                if (v >= n) v -= n;
                if (v == 0) return true;
                if (x < r - 1) {
                    qkd2 = LongNum.PowMod(qkd, 2, n) * 2;
                }
            }
            return false;
        }

        private static LongNum Selfridge(LongNum n) {
            LongNum d;
            for (LongNum d_abs = 5, d_sign = 1; ; d_sign *= -1, d_abs += 2) {
                d = d_abs * d_sign;
                var g = LongNum.Gcd(n, d_abs);

                if (1 < g && g < n) {
                    return null;
                }

                if (Jacobi(d, n) == -1) {
                    return d;
                }
            }
        }

        // алгоритм великий крок - малий крок
        public static LongNum LogBabyStepGiantStep(LongNum a, LongNum b, LongNum n) {
            a %= n;
            b %= n;
            var m = LongNum.Sqrt(n) + 1;
            var g0 = LongNum.PowMod(a, m, n);
            var g = g0;
            var t = new Dictionary<string, LongNum>();

            for (var i = new LongNum(1); i <= m; i++) {
                t.Add(g.ToString(), i);
                g = LongNum.MulMod(g, g0, n);
            }

            for (var j = new LongNum(0); j < m; j++) {
                var y = LongNum.MulMod(b, LongNum.PowMod(a, j, n), n);
                if (t.ContainsKey(y.ToString())) {
                    return m * t[y.ToString()] - j;
                }
            }

            return null;
        }

        // Функція Ейлера
        public static LongNum Phi(LongNum n) {
            if (n < 1) {
                return null;
            }
            
            var res = n;
            for (var i = new LongNum(2); i * i <= n; ++i) {
                if (n % i == 0) {
                    while (n % i == 0) {
                        n /= i;
                    }
                    res -= res / i;
                }
            }

            return n > 1 ? res - res / n : res;
        }


        public static LongNum Lambda(LongNum n) {
            var t = n;
            LongNum r = 0;
            while (t % 2 == 0) {
                t /= 2;
                r++;
            }

            return n > 4 && t == 0 ? Phi(n) / 2 : Phi(n);
        }

        // Функція Мьобіуса
        public static int Mu(LongNum n) {
            if (n == 1)
                return 1;

            var p = new LongNum(0);
            for (var i = new LongNum(2); i <= n; i++) {
                if (n % i == 0 && IsPrime(i)) {
                    if (n % (i * i) == 0)
                        return 0;
                    p++;
                }
            }

            return p % 2 == 0 ? 1 : -1;
        }
        
        // Обчислення символів Лежандра
        public static int? Legendre(LongNum a, LongNum p) {
            if (p < 3 || !IsPrime(p))
                return null;

            if (a % p == 0)
                return 0;

            return LongNum.PowMod(a, (p - 1) / 2, p) == 1 ? 1 : -1;
        }

        // Обчислення символів Якобі
        public static int? Jacobi(LongNum a, LongNum b) {
            if (b < 1 || b % 2 == 0)
                return null;

            if (LongNum.Gcd(a, b) != 1)
                return 0;

            a %= b;
            var t = new LongNum(1);
            while (a != 0) {
                while (a % 2 == 0) {
                    a /= 2;
                    var r = b % 8;
                    if (r == 3 || r == 5)
                        t = -t;
                }
                LongNum.Swap(ref a, ref b);

                if (a % 4 == 3 && b % 4 == 3)
                    t = -t;
                a %= b;
            }
            return b == 1 ? t : new LongNum(0);
        }
        
        // алгоритм Чіполли
        public static (LongNum, LongNum) SqrtCipolla(LongNum n, LongNum p) {
            if (Legendre(n, p) != 1) {
                return (null, 0);
            }

            LongNum a = 0;
            LongNum w2;
            while (true) {
                w2 = (a * a + p - n) % p;
                if (Legendre(w2, p) != 1)
                    break;
                a++;
            }

            var finalW = w2;
            (LongNum, LongNum) MulExtended((LongNum, LongNum) aa, (LongNum, LongNum) bb) {
                return ((aa.Item1 * bb.Item1 + aa.Item2 * bb.Item2 * finalW) % p,
                        (aa.Item1 * bb.Item2 + bb.Item1 * aa.Item2) % p);
            }

            var r = (new LongNum(1), new LongNum(0));
            var s = (a, new LongNum(1));
            var nn = (p + 1) / 2;
            while (nn > 0) {
                if (nn % 2 != 0) {
                    r = MulExtended(r, s);
                }
                s = MulExtended(s, s);
                nn /= 2;
            }

            if (r.Item2 != 0 || r.Item1 * r.Item1 % p != n) {
                return (0, null);
            }

            return (r.Item1, p - r.Item1);
        }
        
        // алгоритм Соловея-Штрассена
        public static string IsPrimeSolovayStrassen(LongNum n, int k) {
            if (n < 2) {
                return "The number " + n + " is less than 2, hence isn't prime.";
            }

            if (n == 2) {
                return "The number 2 is prime.";
            }

            if (n % 2 == 0) {
                return "The number " + n + " is even, hence isn't prime.";
            }

            for (int i = 0; i < k; i++) {
                var a = LongNum.Rand(2, n);
                if (LongNum.Gcd(a,n) > 1 || LongNum.PowMod(a, (n - 1) / 2, n) != Legendre(a, n) % n) {
                    return "The number " + n + " is odd and isn't prime.";
                }
            }

            double prob = 1 - Math.Pow(2, -k);
            return "The number " + n + " is prime with propability " + prob + ".";
        }

        // Криптосистеми Ель-Гамаля
        public static void ElGamal(EllipticCurve curve) {
            // Step 1. Bob chooses a random number k = 1, ..., N-1.
            var k = LongNum.Rand(1, curve.N);
            Console.WriteLine("Bob's private key: " + k.ToString());
            var Y = curve.PointSelfSum(k, curve.G);

            // Step 2. Alice's message
            (LongNum x, LongNum y) M;

            // If there was a way to quickly compute Y-s on my computer,
            // here would've been the encoding of message as the elliptic curve point.

            /*Console.Write("Enter Alice's message: ");
            M.x = MessageToLongNum(Console.ReadLine());
            M.y = SqrtCipolla(Pow(M.x, 3) + curve.A * M.x + curve.B, curve.P).Item1;*/

            Console.Write("Enter point's x coordinate: ");
            M.x = new LongNum(Console.ReadLine());
            Console.Write("Enter point's y coordinate: ");
            M.y = new LongNum(Console.ReadLine());

            // Step 3. Encryption
            var r = LongNum.Rand(1, curve.N);
            var D = curve.PointSelfSum(r, Y);
            var G = curve.PointSelfSum(r, curve.G);
            var H = curve.AddPoints(M, D);

            // Step 4. Decryption
            var S = curve.PointSelfSum(k, G);
            var S1 = (S.Item1, (S.Item1 + S.Item2) % curve.P);
            var M1 = curve.AddPoints(S1, H);
            Console.WriteLine("Point decrypted: (" + M1.Item1.ToString() + "," + M1.Item2.ToString() + ")");

            // Message processing again
            /*var res = LongNumToMessage(M1.Item1);
            Console.WriteLine("Message decrypted: " + res);*/
        }

        #region Inner methods
        public static bool IsPrime(LongNum n) {
            if (n == 2)
                return true;

            if (n % 2 == 0)
                return false;

            var t = LongNum.Sqrt(n);
            for (LongNum k = 3; k <= t; k += 2) {
                if (n % k == 0)
                    return false;
            }

            return true;
        }
        private static LongNum F(LongNum x, LongNum n) => (x * x + 1) % n;

        public static LongNum MessageToLongNum(string msg) {
            LongNum res = 0;
            for (int k = msg.Length - 1; k >= 0; k--) {
                var cur = msg[msg.Length - k - 1];
                var c = cur == ' ' ? 0 : cur - ALPH_DIFF;
                res += c * LongNum.Pow(ALPH_BASE, k);
            }

            return res;
        }

        public static string LongNumToMessage(LongNum n) {
            string res = "";
            int r;
            do {
                r = n % ALPH_BASE;
                res += r == 0 ? ' ' : (char)(r + ALPH_DIFF);
                n /= ALPH_BASE;
            } while (n >= ALPH_BASE);
            r = n;
            res += r == 0 ? ' ' : (char)(r + ALPH_DIFF);

            var arr = res.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        public static LongNum HexToDecimal(string n) {
            const int diff = 55; // 'A' ascii code - 'A' hex number
            const int A = 65;
            const int F = 70;

            LongNum res = 0;
            var len = n.Length;
            var hex = new LongNum(16);

            for (int i = 0; i < len; i++) {
                var d = (int)n[i];
                if (d >= A && d <= F) {
                    d -= diff;
                }
                else {
                    d = int.Parse(n[i].ToString());
                }

                var k = n.Length - i - 1;
                res += d * LongNum.Pow(hex, k); 
            }

            return res;
        }
        #endregion
    }
}