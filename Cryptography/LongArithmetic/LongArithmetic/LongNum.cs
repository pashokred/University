using System;
using System.Collections.Generic;
using System.Linq;

namespace LongArithmetic {
    public partial class LongNum {
        protected bool Equals(LongNum other)
        {
            return _sign == other._sign && Equals(Digits, other.Digits);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((LongNum)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_sign, Digits);
        }

        #region Fields
        const int Base = 10;
        private bool _sign;
        public List<int> Digits { get; set; } = new();   // Reverse order!
        public bool Sign { 
            get => _sign;
            set => _sign = (Digits.Count != 1 || Digits[0] != 0) && value;
        }
        #endregion

        #region Constructors
        public LongNum() { }

        public LongNum(string number) {
            Sign = number[0] == '-';
            for (int i = number.Length - 1; i > number.IndexOf('-'); i--)
                Digits.Add(number[i] - '0');
        }

        public LongNum(int number) {
            Sign = number < 0;
            number = Math.Abs(number);

            do {
                Digits.Add(number % Base);
                number /= Base;
            } while (number > 0);
        }
        #endregion

        #region Comparison operators
        public static bool operator ==(LongNum a, LongNum b) => a.Sign == b.Sign && a.Digits.SequenceEqual(b.Digits);

        public static bool operator !=(LongNum a, LongNum b) => !(a == b);

        public static bool operator <(LongNum a, LongNum b) {
            if (a.Sign != b.Sign)
                return a.Sign;

            if (a.Digits.Count != b.Digits.Count)
                return a.Sign ? a.Digits.Count > b.Digits.Count : a.Digits.Count < b.Digits.Count;

            // if signs and counts equal 
            for (int i = a.Digits.Count; i >= 0; i--)
                if (a[i] != b[i])
                    return a.Sign ? a[i] > b[i] : a[i] < b[i];

            return false;
        }

        public static bool operator <=(LongNum a, LongNum b) => a == b || a < b;

        public static bool operator >(LongNum a, LongNum b) => !(a <= b);

        public static bool operator >=(LongNum a, LongNum b) => !(a < b);

        #endregion

        #region Unary operators
        public static LongNum operator +(LongNum a) => new() { Digits = a.Digits, Sign = a.Sign };

        public static LongNum operator -(LongNum a) => new() { Digits = a.Digits, Sign = !a.Sign };

        public static LongNum operator ++(LongNum a) => a + 1;

        public static LongNum operator --(LongNum a) => a - 1;
        #endregion

        #region Binary operators
        public static LongNum operator +(LongNum a, LongNum b) {
            var res = new LongNum();

            if (a.Sign == b.Sign) {
                AddWithSameSign(a, b, ref res);
            }
            else {
                res = a.Sign ? b - (-a) : a - (-b);
            }

            return res;
        }

        public static LongNum operator -(LongNum a, LongNum b) {
            if (a == b) return 0;

            var res = new LongNum();

            if (a.Sign == b.Sign) {
                SubWithSameSign(a, b, ref res);
            }
            else {
                res = a.Sign ? -(-a + b) : a + (-b);
            }

            return res;
        }

        public static LongNum operator *(LongNum a, LongNum b) {
            if (a == 0 || b == 0)
                return 0;

            // a must have more or eq number of digits
            if (a.Digits.Count < b.Digits.Count)
                Swap(ref a, ref b);

            var blocks = FormMulBlocks(a, b);

            return AddMulBlocks(blocks, a, b);
        }

        public static LongNum operator /(LongNum a, LongNum b) {
            // check constants
            if (b == 0)
                return null;
            if (a == 0)
                return 0;
            if (b == 1)
                return a;
            if (b == -1)
                return -a;

            var subA = new LongNum { Sign = false };
            var res = ColumnDivide(a, b, ref subA);
            var modIs0 = subA != 0 && subA.Digits.Count != 0;

            if (a.Sign && b.Sign && modIs0)
                res++;
            if (a.Sign && !b.Sign && modIs0)
                res--;
            if (res[0] == 0)
                res.Sign = false;

            return res;
        }

        public static LongNum operator %(LongNum a, LongNum b) {
            if (b == 0)
                return null;

            var r = a - b * (a / b);
            return r == b ? new LongNum(0) : r;
        }
        #endregion

        #region Mathematical functions

        public static LongNum Abs(LongNum a) {
            return new LongNum { Digits = a.Digits, Sign = false };
        }

        public static LongNum Pow(LongNum a, LongNum n) {
            if (n < 0)
                return a == 0 ? null : new LongNum(0);
            if (n == 0)
                return 1;
            if (n == 1 || a == 0)
                return a;

            var res = new LongNum(1);

            while (n > 0) {
                if (n % 2 == 1)
                    res *= a;
                a *= a;
                n /= 2;
            }

            return res;
        }

        public static LongNum Sqrt(LongNum a) {
            if (a.Sign)
                return null;
            if (a < 4)
                return a == 0 ? 0 : 1;

            var k = 2 * Sqrt((a - a % 4) / 4);
            return a < Pow((k + 1), 2) ? k : k + 1; 
        }

        public static LongNum Gcd(LongNum a, LongNum b) {
            a = Abs(a);
            b = Abs(b);

            while (b != 0) {
                b = a % (a = b);
            }
            return a;
        }

        public static (LongNum K1, LongNum K2) GcdLinearRepresentation(LongNum a, LongNum b) {
            if (a == 0)
                return (0, 1);
            if (b == 0)
                return (1, 0);

            var q = a / b;
            var r = a - q * b;

            if (r == 0)
                return (1, 1 - q);

            a = b;
            b = r;
            var u = new LongNum(1);
            var u1 = new LongNum(1);
            var u2 = new LongNum(0);
            var v = -q;
            var v1 = -q;
            var v2 = new LongNum(1);

            while (a % b != 0) {
                q = a / b;
                r = a - q * b;
                a = b;
                b = r;
                u = -q * u1 + u2;
                v = -q * v1 + v2;
                u2 = u1;
                u1 = u;
                v2 = v1;
                v1 = v;
            }

            if (u.Sign && u == 0)
                u.Sign = false;
            if (v.Sign && v == 0)
                v.Sign = false;

            return (u, v);
        }

        #region Functions by modulo
        public static LongNum AddMod(LongNum a, LongNum b, LongNum m) => (a + b) % m;

        public static LongNum SubMod(LongNum a, LongNum b, LongNum m) => (a - b) % m;

        public static LongNum MulMod(LongNum a, LongNum b, LongNum m) => (a * b) % m;

        public static LongNum DivMod(LongNum a, LongNum b, LongNum m) {
            var d = a / b;
            return d is null ? null : (a / b) % m;
        }

        public static LongNum ModMod(LongNum a, LongNum b, LongNum m) {
            var d = a % b;
            return d is null ? null : (a % b) % m;
        }

        public static LongNum PowMod(LongNum a, LongNum b, LongNum m) {
            if (b < 0)
                return null;
            if (b == 0)
                return 1;
            if (b == 1 || a == 0)
                return a % m;

            var res = new LongNum(1);

            while (b > 0) {
                if (b % 2 == 1)
                    res = (res * a) % m;
                a = (a * a) % m;
                b /= 2;
            }

            return res;
        }
        #endregion

        #endregion

        #region Congruences

        public static LongNum MulInverse(LongNum a, LongNum m) => GcdLinearRepresentation(a, m).K1;

        public static (LongNum r1, LongNum r2) SolveCongruence(LongNum a, LongNum b, LongNum m) {
            if (m == 0) {
                return (null, null);
            }
            if (b == 0) {
                if (a == 0)
                    return (0, 1);
                return (0, m);
            }
            if (a < 0 || a >= m)
                a %= m;
            if (b < 0 || b >= m)
                b %= m;

            var d = Gcd(a, m);
            if (b % d != 0)
                return (null, null);

            var (k1, _) = GcdLinearRepresentation(a, m);
            var f = b / d;
            return (k1 * f, m / d);
        }

        public static void NormalizeCongruenceSol(ref (LongNum r1, LongNum r2) sol) => sol.r1 %= sol.r2;

        public static (LongNum r1, LongNum r2) SolveCongruenceSystem(LongNum[] a, LongNum[] b, LongNum[] m) {
            if (a.Length != b.Length || a.Length != m.Length || b.Length != m.Length) {
                return (null, null);
            }

            var sols = new (LongNum r1, LongNum r2)[a.Length];
            for (int i = 0; i < sols.Length; i++) {
                sols[i] = SolveCongruence(a[i], b[i], m[i]);

                if (sols[i].r1 is null && sols[i].r2 is null) {
                    return (null, null);
                }
            }

            var prevSol = sols[0];
            NormalizeCongruenceSol(ref prevSol);
            for (int i = 1; i < sols.Length; i++) {
                var r1 = prevSol.r1;
                var r2 = prevSol.r2; // prev: x = r1 (mod r2)
                var bi = sols[i].r1;
                var mi = sols[i].r2; // current: x = bi (mod mi)
                var sol = SolveCongruence(r2 % mi, (bi - r1) % mi, mi);

                if (sol.r1 is null) {
                    return (null, null);
                }

                NormalizeCongruenceSol(ref sol);
                var mr = r2 * sol.r2;
                prevSol = ((r1 + r2 * sol.r1) % mr, mr);
            }

            return prevSol;
        }

        public static void OutputCongruenceSol(LongNum r1, LongNum r2) {
            Console.WriteLine("Solution is x = " + r1 + " + " + r2 + "k");
            Console.WriteLine("Alternate form: x = " + r1 + " (mod " + r2 + ")");
        }

        public static void OutputCongruenceSol(LongNum r1, LongNum r2, LongNum m) {
            OutputCongruenceSol(r1, r2);
            Console.Write("Solutions in Z" + m + ": x = ");

            var sols = "";
            var d = m / r2;

            for (var k = new LongNum(0); k < d; k++)
            {
                var x = (r1 + r2 * k) % m;
                sols += x + ", ";
            }
            sols = sols.Substring(0, sols.Length - 2);
            Console.Write(sols + "\n");
        }

        #endregion

        #region Utility methods

        public int this[int i] {
            get => i < Digits.Count ? Digits[i] : 0;
            set => Digits[i] = value;
        }

        public override string ToString() {
            string result = Sign ? "-" : "";
            for (int i = Digits.Count - 1; i >= 0; i--)
                result += Digits[i];
            return result;
        }

        public static void Swap(ref LongNum a, ref LongNum b) {
            (a, b) = (b, a);
        }

        public void ClearZeros() {
            int c = Digits.Count - 1;
            while (c > 0 && Digits[c] == 0)
                Digits.RemoveAt(c--);
        }

        public static LongNum Rand(LongNum a, LongNum b) {
            var rnd = new Random();
            var res = new LongNum();
            var len = rnd.Next(a.Digits.Count, b.Digits.Count + 1);

            if (len == 1)
                res.Digits.Add(rnd.Next(0, Base));
            else
                res.Digits.Add(rnd.Next(1, Base));

            for (int i = 1; i < a.Digits.Count; i++) {
                res.Digits.Add(rnd.Next(a[i], Base));
            }

            var eq = len == b.Digits.Count;
            for (int i = 0; i < len - a.Digits.Count; i++) {
                var d = eq ? rnd.Next(0, b[a.Digits.Count + i]) : rnd.Next(0, Base);
                res.Digits.Add(d);
            }

            return res;
        }

        public static implicit operator LongNum(int n) => new LongNum(n);

        public static implicit operator int(LongNum n) => int.Parse(n.ToString());
        #endregion
    }
}