using LongArithmetic;

namespace Crypto {
    public class EllipticCurve {
        // y^2 = x^3 + Ax + B (mod P)
        public LongNum P { get; set; }
        public LongNum A { get; set; }
        public LongNum B { get; set; }
        public LongNum N { get; set; }                     // Order of the curve
        public (LongNum x, LongNum y) G { get; set; }   // Generator (base point)


        public EllipticCurve(LongNum p, LongNum a, LongNum b, LongNum n, LongNum gx, LongNum gy) {
            P = p;
            A = a;
            B = b;
            G = (gx, gy);
            N = n;
        }

        public (LongNum, LongNum) AddPoints(LongNum x1, LongNum y1, LongNum x2, LongNum y2) {
            if (!IsPointOnCurve(x1, y1) || !IsPointOnCurve(x2, y2)) {
                return (null, null);
            }

            if (PointsEqual(ref x1, ref y1, ref x2, ref y2)) {
                if (y1 == 0 || y1 == -1) {
                    return (-1, -1);
                }

                var m = LongNum.MulMod(3 * x1 * x1 + A, LongNum.MulInverse(2 * y1, P), P);
                var x3 = (m * m - 2 * x1) % P;
                return (x3, (-y1 + m * (x1 - x3)) % P);
            }
            else {
                if (x1 == x2) {
                    return (-1, 1);
                }

                if (x1 == -1) {
                    return (x2, y2);
                } 

                if (x2 == -1) {
                    return (x1, y1);
                }

                var m = LongNum.MulMod(y2 - y1, LongNum.MulInverse(x2 - x1, P), P);
                var x3 = (m * m - x1 - x2) % P;
                return (x3, (-y1 + m * (x1 - x3)) % P);
            }
        }

        public (LongNum, LongNum) AddPoints((LongNum, LongNum) p1, (LongNum, LongNum) p2) {
            return AddPoints(p1.Item1, p1.Item2, p2.Item1, p2.Item2);
        }

        private bool IsPointOnCurve(LongNum x, LongNum y) {
            return x == -1 && y == -1 ||
                   x >= 0 && y >= 0 && x < P && y < P &&
                   (y * y) % P == (LongNum.Pow(x, 3) + A * x + B) % P;
        }

        private bool PointsEqual(ref LongNum x1, ref LongNum y1, ref LongNum x2, ref LongNum y2) {
            return x1 == x2 && y1 == y2;
        }

        public (LongNum, LongNum) PointSelfSum(LongNum k, (LongNum, LongNum) p) {
            var res = p;
            for (LongNum i = 1; i < k; i++) {
                res = AddPoints(res, p);
            }

            return res;
        }

        public override string ToString() {
            var aStr = A == 0 ? "" : A == 1 ? " + x" : " + " + A.ToString() + "x";
            var bStr = B == 0 ? "" : " + " + B.ToString();
            return "y^2 = x^3" + aStr + bStr + " (mod " + P + ")";
        }
    }
}