using System;
using System.Linq;

namespace LongArithmetic
{
    public partial class LongNum
    {
        private static void AddWithSameSign(LongNum a, LongNum b, ref LongNum res) {
            int extra = 0;
            res.Sign = a.Sign;

            for (int i = 0; i < Math.Max(a.Digits.Count, b.Digits.Count); i++) {
                int dSum = a[i] + b[i] + extra;
                res.Digits.Add(dSum % Base);
                extra = dSum / Base;
            }

            if (extra == 1) res.Digits.Add(1);
        }

        private static void SubWithSameSign(LongNum a, LongNum b, ref LongNum res) {
            int extra = 0;

            if (a < b) {
                for (int i = 0; i < Math.Max(a.Digits.Count, b.Digits.Count); i++) {
                    var dDif = a.Sign ? a[i] - b[i] - extra : b[i] - a[i] - extra;

                    if (dDif < 0) {
                        dDif += Base;
                        extra = 1;
                    }
                    else
                        extra = 0;

                    res.Digits.Add(dDif);
                }
                res.Sign = true;
            }
            else
                res = -(b - a);

            res.ClearZeros();
        }

        private static LongNum[] FormMulBlocks(LongNum a, LongNum b) {
            var blocks = new LongNum[b.Digits.Count];

            for (int i = 0; i < blocks.Length; i++) {
                int pCarry = 0;
                blocks[i] = new LongNum();
                
                for (int j = 0; j < i; j++)
                    blocks[i].Digits.Add(0);
                
                for (int j = 0; j < a.Digits.Count; j++) {
                    int dProd = a[j] * b[i] + pCarry;
                    blocks[i].Digits.Add(dProd % Base);
                    pCarry = dProd / Base;
                }

                if (pCarry > 0)
                    blocks[i].Digits.Add(pCarry);
            }

            return blocks;
        }

        private static LongNum AddMulBlocks(LongNum[] blocks, LongNum a, LongNum b) {
            var res = new LongNum { Sign = a.Sign != b.Sign };
            int sCarry = 0;

            for (int i = 0; i < blocks[^1].Digits.Count; i++) {
                int sum = sCarry + blocks.Sum(t => t[i]);

                res.Digits.Add(sum % Base);
                sCarry = sum / Base;
            }

            if (sCarry > 0) {
                while (sCarry > 0) {
                    res.Digits.Add(sCarry % Base);
                    sCarry /= Base;
                }
            }

            return res;
        }

        private static LongNum ColumnDivide(LongNum a, LongNum b, ref LongNum subA) {
            var res = new LongNum { Sign = a.Sign != b.Sign };
            var absB = Abs(b);
            var i = a.Digits.Count - 1;
            var firstStep = true;

            while (i >= 0) {
                int added = 0;

                do {
                    subA.Digits.Insert(0, a[i]);
                    i--;
                    added++;

                    subA.ClearZeros();

                    if (added > 1 && !firstStep)
                        res.Digits.Insert(0, 0);
                } while (subA < absB && i >= 0);

                if (firstStep)
                    firstStep = false;

                ModifyMinuend(ref subA, absB, ref res);
            }

            return res;
        }

        private static void ModifyMinuend(ref LongNum subA, LongNum absB, ref LongNum res) {
            var quot = NaiveDiv(subA, absB);
            res.Digits.Insert(0, quot);
            subA -= absB * quot;

            if (subA == 0)
                subA.Digits.Remove(0);
        }

        public static LongNum NaiveDiv(LongNum a, LongNum b) {
            var res = new LongNum(0);

            while (a >= b) {
                a -= b;
                res++;
            }
            return res;
        }
    }
}