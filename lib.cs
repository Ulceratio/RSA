using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACipher
{
    class lib
    {
        public struct Remainder
        {
            public int a { get; set; }
            public int m { get; set; }
        }

        public struct Factor
        {
            public int q { get; set; }
            public int alpha { get; set; }
        }

        public decimal ModPow(decimal alpha, decimal beta, decimal n)  // (alpha ^ beta) mod n
        {
            decimal res = 1;
            while (beta != 0)
            {
                if ((beta % 2) != 0)//beta%2!=0
                {
                    res = (res * alpha) % n;
                    beta -= 1;
                }
                alpha = (alpha * alpha) % n;
                beta /= 2; // beta/=2;
            }
            return res;
        }


        public int logA(int a, int b, int p)
        {
            int x = 0;
            while (true)
            {
                if (ModPow(a, x, p) == b)
                {
                    return x;
                }
                else
                {
                    x++;
                }
            }
        }

        public int BinPow(int alpha, int beta)  // (alpha ^ beta)
        {
            int res = 1;
            while (beta != 0)
            {
                if ((beta & 1) != 0)//beta%2!=0
                {
                    res = (res * alpha);
                    beta -= 1;
                }
                alpha = (alpha * alpha);
                beta = beta >> 1; // beta/=2;
            }
            return res;
        }

        public int logB(int alpha, int beta, int n) // alpha ^ x = beta (mod n)
        {
            int x = 0;
            int k = Convert.ToInt32(Math.Sqrt(n));
            int[] litleSteps = new int[k];
            int[] bigSteps = new int[k];
            for (int i = 0, j = 1; i < k; i++, j++)
            {
                bigSteps[i] = Convert.ToInt32(ModPow(alpha, j * k, n).ToString());
                litleSteps[i] = (Convert.ToInt32(ModPow(alpha, j, n).ToString()) * beta) % n;
            }
            int sameEl = 0;
            try
            {
                sameEl = Convert.ToInt32(string.Join("\n", bigSteps.Intersect(litleSteps)));
            }
            catch
            {
                for (int i = 0; i < bigSteps.Length; i++)
                {
                    for (int j = 0; j < litleSteps.Length; j++)
                    {
                        if (bigSteps[i] == litleSteps[j])
                        {
                            sameEl = bigSteps[i];
                            break;
                        }
                    }
                }
            }
            x = (Array.IndexOf(bigSteps, sameEl) + 1) * k - (Array.IndexOf(litleSteps, sameEl) + 1);
            return x;
        }

        public decimal inverseElement(decimal a, decimal mod)
        {
            decimal result = ((gcdex((Int64)a, (Int64)mod).Item2 % mod) + mod) % mod;
            return result;
        }

        public Tuple<long, long, long> gcdex(long m, long n)
        {
            long u = 0;
            long v = 0;
            long NOD = 0;
            long a = m;
            long b = n;
            long u1 = 1, v1 = 0, u2 = 0, v2 = 1;
            long q, r;
            long temp;
            while (b != 0)
            {
                q = a / b;
                r = a % b;
                a = b;
                b = r;
                temp = u2; u2 = u1 - q * u2; u1 = temp;
                temp = v2; v2 = v1 - q * v2; v1 = temp;
                a = u1 * m + v1 * n;
                b = u2 * m + v2 * n;
            }
            NOD = a; u = u1; v = v1;
            return Tuple.Create(NOD, u, v);
        }

        public int ChineseReminderTheorem(List<Remainder> equation)
        {
            int result = 0;
            int M = 1;

            foreach (var item in equation)
            {
                M *= item.m;
            }

            for (int i = 0; i < equation.Count; i++)
            {
                result += (int)(equation[i].a * (M / equation[i].m) * inverseElement((M / equation[i].m), equation[i].m));
            }

            return result % M;
        }

        public List<Factor> factorizationOfTheNumber(int p)
        {
            List<Factor> result = new List<Factor>();
            foreach (var item in getSimpleNumbersEratosthenes(p))
            {
                result.Add(new Factor { q = item, alpha = 0 });
            }
            Parallel.For(1, result.Count, (i, state) =>
            {
                Factor temp = new Factor();
                while ((p % result[i].q) == 0)
                {
                    temp.q = result[i].q;
                    temp.alpha = result[i].alpha + 1;
                    result[i] = temp;
                    p /= result[i].q;
                }
            }
            );
            result = result.Where(x => x.alpha != 0).ToList();
            return result;
        }

        public List<int> getSimpleNumbersEratosthenes(int thisTaskValue)
        {
            bool[] result = new bool[thisTaskValue + 1];
            for (int i = 0; i < thisTaskValue + 1; i++)
            {
                result[i] = true;
            }
            int p = 2;
            while (p * p < thisTaskValue)
            {
                for (int i = 2; p * i <= thisTaskValue; i++)
                {
                    result[p * i] = false;
                }

                for (int i = p; i <= thisTaskValue; i++)
                {
                    if (result[i] == true && i > p)
                    {
                        p = i;
                        break;
                    }
                }
            }
            List<int> resultNums = new List<int>();
            for (int i = 1; i < result.Length; i++)
            {
                if (result[i])
                {
                    resultNums.Add(i);
                }
            }
            return resultNums;
        }

        public int PohligHellmanAlgorithm(int a, int b, int p) // a ^ x = b (mod p)
        {
            List<List<int>> r = new List<List<int>>();
            List<Factor> factors = factorizationOfTheNumber(p - 1);
            foreach (var item in factors)
            {
                Console.WriteLine(item.q + " " + item.alpha);
            }
            for (int i = 0; i < factors.Count; i++)
            {
                List<int> tmp = new List<int>();
                r.Add(tmp);
                for (int j = 0; j <= factors[i].q - 1; j++)
                {
                    tmp.Add((int)ModPow(a,
                        (j * ((p - 1) / factors[i].q)),
                        p));
                }
            }
            foreach (var item in r)
            {
                foreach (var item1 in item)
                {
                    Console.Write(item1 + " ");
                }
                Console.WriteLine();
            }
            List<Remainder> remainders = new List<Remainder>();
            Parallel.For(0, factors.Count, (i, state) =>
            {
                List<int> x = new List<int>();
                int pow = 0;
                int temp = 0;
                for (int xi = 0; xi <= factors[i].q - 1; xi++)
                {
                    for (int j = 0, k = 0; j < x.Count; j++, k++)
                    {
                        if (j >= 0)
                        {
                            pow -= x[j] * Convert.ToInt32(BinPow(factors[i].q, j));
                        }
                        else
                        {
                            pow -= x[j] * Convert.ToInt32(Math.Pow(factors[i].q, j));
                        }
                        // Console.WriteLine("j={0}", xi);
                    }
                    temp = (int)ModPow((int)(b * Math.Pow(a, pow)),
                        (int)((p - 1) / Math.Pow(factors[i].q, xi + 1)),
                        p);
                    //  Console.WriteLine("temp={0}", temp);
                    for (int j = 0; j < r.ElementAt(i).Count; j++)
                    {
                        if (r.ElementAt(i).ElementAt(j) == temp)
                        {
                            x.Add(j);
                            break;
                        }
                    }
                }
                Remainder nRemainder = new Remainder();
                nRemainder.a = 0;
                nRemainder.m = BinPow(factors[i].q, factors[i].alpha);
                for (int j = 0; j < x.Count; j++)
                {
                    if (j >= 0)
                    {
                        nRemainder.a += x[j] * Convert.ToInt32(BinPow(factors[i].q, j));
                    }
                    else
                    {
                        nRemainder.a += x[j] * Convert.ToInt32(Math.Pow(factors[i].q, j));
                    }
                    Console.WriteLine("i={0}  j={1}", i, j);
                }
                remainders.Add(nRemainder);
            }
            );
            foreach (var item in remainders)
            {
                Console.WriteLine("a={0} m={1}", item.a, item.m);
            }
            int result = ChineseReminderTheorem(remainders);
            return result;
        }
    }
}

