using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RSACipher
{
    class RSA
    {
        decimal n;
        decimal p;
        decimal q;
        int e = 65537;
        decimal phi;
        decimal d;

        public RSA()
        {
            Random nR = new Random(1539827);
            do
            {
                p = nR.Next(100000, 120000);
            }
            while (!testPrimeNumbers(p, 5));
            for (; ; )
            {
                q = nR.Next(100000, 120000);
                if (testPrimeNumbers(q, 5))
                {
                    if (q != p)
                    {
                        break;
                    }
                }
            }
            n = p * q;
            phi = (p - 1) * (q - 1);
            lib newLib = new lib();
            d = newLib.inverseElement(e, phi);
        }

        public string EncryptionOneByte(byte[] fileToEncrypt)
        {
            Console.WriteLine("Этап 1, создание массива decimal");
            decimal[] result = new decimal[fileToEncrypt.Length];
            lib newLib = new lib();
            Console.WriteLine("Этап 2, шифрование");
            Parallel.For(0, result.Length, (i, state) =>
            {

                result[i] = newLib.ModPow(Convert.ToDecimal(fileToEncrypt[i]), e, n);
            }
                );
            Console.WriteLine("Этап 3, запись в строку");
            string resultStr = String.Join("|", result);
            return resultStr;
        }

        public void EncryptionFourBytes(byte[] fileToEncrypt, string path)
        {
            Console.WriteLine("Этап 1, добавление элементов");
            fileToEncrypt = AddElements(fileToEncrypt);
            Console.WriteLine("Этап 2, создание массива decimal");
            decimal[] result = makeArrFromByteArr(fileToEncrypt);
            lib newLib = new lib();
            Console.WriteLine("Этап 3, шифрование");
            Parallel.For(0, result.Length, (i, state) =>
            {
                result[i] = newLib.ModPow(result[i], e, n);
            }
                );
            Console.WriteLine("Этап 4, запись в строку");
            string resultStr = String.Join("|", result);
            File.WriteAllText(path, resultStr);
        }

        public decimal[] makeArrFromByteArr(byte[] arr)
        {
            decimal[] result = new decimal[arr.Length / 4];
            for (int i = 0, j = 0; j < result.Length; i += 4, j++)
            {
                result[j] = byteMerge(arr[i], arr[i + 1], arr[i + 2], arr[i + 3]);
            }
            return result;
        }


        public string DecryptionOneByte(string fileToDecrypt, string path)
        {
            Console.WriteLine("Этап 1,запись в массив decimal");
            decimal[] result = fileToDecrypt.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Select(x => decimal.Parse(x)).ToArray();
            lib newLib = new lib();
            Console.WriteLine("Этап 2,дешифрование");
            Parallel.For(0, result.Length, (i, state) =>
            {
                result[i] = newLib.ModPow(result[i], d, n);
            }
                );
            Console.WriteLine("Этап 3,запись в массив байт");
            byte[] resByteArr = new byte[result.Length];
            for (int i = 0; i < resByteArr.Length; i++)
            {
                resByteArr[i] = Convert.ToByte(result[i]);
            }
            Console.WriteLine("Этап 3 ,запись в файл");
            File.WriteAllBytes(@path, resByteArr);
            return null;
        }

        public void DecryptionFourBytes(string fileToDecrypt, string path)
        {
            Console.WriteLine("Этап 1,запись в массив decimal");
            decimal[] result = fileToDecrypt.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries).Select(x => decimal.Parse(x)).ToArray();
            lib newLib = new lib();
            Console.WriteLine("Этап 2,дешифрование");
            Parallel.For(0, result.Length, (i, state) =>
            {
                result[i] = newLib.ModPow(result[i], d, n);
            }
                );
            Console.WriteLine("Этап 3,запись в массив байт");
            byte[] resByteArr = new byte[result.Length * 4];
            for (int i = 0, j = 0; i < result.Length; i++, j += 4)
            {
                Tuple<byte, byte, byte, byte> tempTuple = divideBytes((uint)result[i]);
                resByteArr[j] = tempTuple.Item1;
                resByteArr[j + 1] = tempTuple.Item2;
                resByteArr[j + 2] = tempTuple.Item3;
                resByteArr[j + 3] = tempTuple.Item4;
            }
            Console.WriteLine("Этап 3,удаление добавленных элементов");
            resByteArr = RemoveElements(resByteArr);
            Console.WriteLine("Этап 4 ,запись в файл");
            File.WriteAllBytes(@path, resByteArr);
        }

        public bool testPrimeNumbers(decimal n, decimal k)
        {
            bool result = true;
            Random nR = new Random(16573);
            lib newLib = new lib();
            decimal a;
            decimal temp1 = n - 1;
            decimal s = 0;
            decimal t = 0;
            while (temp1 % 2 == 0)
            {
                s++;
                temp1 /= 2;
            }
            t = temp1;
            decimal x = 0;
            for (int i = 0; i <= k; i++)
            {
                a = nR.Next(2, (int)(n - 1));
                x = newLib.ModPow(a, t, n);
                if (x == 1 || x == n - 1)
                {
                    continue;
                }
                for (int j = 0; j < s - 1; j++)
                {
                    x = Convert.ToInt32(newLib.ModPow(x, 2, n));
                    if (x == 1)
                    {
                        result = false;
                        break;
                    }
                    if (x == n - 1)
                    {
                        break;
                    }
                }
                if (x != n - 1)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }

        public byte[] getSequence()
        {
            return new byte[] { 251, 241, 239, 233, 229, 227, 223, 211, 199, 197, 193, 191, 181, 179, 173, 167, 163, 157, 151, 149, 139, 137, 131, 127, 113, 109, 107, 103, 101, 97, 89, 83, 79, 73, 71, 67, 61, 59, 53, 47, 43, 41, 37, 31, 29, 23, 19, 17, 13, 11, 7, 5, 3, 2, 1 };
        }

        public byte[] reverseSequence(byte[] seq)
        {
            byte t;
            for (int i = 0, j = seq.Length - 1; i < j; i++, j--)
            {
                t = seq[i];
                seq[i] = seq[j];
                seq[j] = t;
            }
            return seq;
        }

        public byte[] AddElements(byte[] curArr)
        {
            if (curArr.Length % 4 != 0)
            {
                int q = this.getSequence().Length + curArr.Length + 1;
                while (true)
                {
                    if (q % 4 != 0)
                    {
                        q--;
                    }
                    else
                    {
                        break;
                    }
                }
                int lngth = q - curArr.Length;
                byte[] res = new byte[lngth];
                byte[] seq = this.reverseSequence(this.getSequence());
                res[0] = (byte)(q - curArr.Length);
                for (int i = 0, j = 1; j < res.Length; i++, j++)
                {
                    res[j] = seq[i];
                }
                byte[] base_byte = res.Concat(curArr).ToArray();
                return base_byte;
            }
            else
            {
                int q = this.getSequence().Length + curArr.Length;
                byte[] res = new byte[this.getSequence().Length + 1];
                res[0] = 56;
                byte[] seq = this.reverseSequence(this.getSequence());
                for (int i = 0, j = 1; j < res.Length; i++, j++)
                {
                    res[j] = seq[i];
                }
                byte[] result = res.Concat(curArr).ToArray();
                return result;
            }
        }

        public byte[] RemoveElements(byte[] curArr)
        {
            byte[] result;
            int needToDelete = curArr[0] - 1;
            int counter = 0;
            byte[] seq = this.reverseSequence(this.getSequence());
            for (int i = 1, j = 0; i < curArr.Length; i++, j++)
            {
                if (curArr[i] == seq[j] && counter < needToDelete)
                {
                    counter++;
                }
                else
                {
                    break;
                }
            }
            if (counter != needToDelete)
            {
                throw new Exception("Encryption failed");
            }
            else
            {
                result = new byte[curArr.Length - (needToDelete + 1)];
                Array.Copy(curArr,
                (needToDelete + 1),
                result,
                0,
                curArr.Length - (needToDelete + 1));
                return result;
            }
        }
        public uint byteMerge(byte b1, byte b2, byte b3, byte b4)
        {
            return ((uint)b1) << 24 | ((uint)b2) << 16 | ((uint)b3) << 8 | ((uint)b4);
        }
        public Tuple<byte, byte, byte, byte> divideBytes(UInt32 val)
        {
            return new Tuple<byte, byte, byte, byte>((byte)(val >> 24), (byte)((val >> 16) | (val << 8)), (byte)((val >> 8) | (val << 16)), (byte)(val & ~((~((uint)0)) << 8)));
        }
    }
}
