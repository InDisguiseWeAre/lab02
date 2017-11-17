using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab01_Magma
{
    class Program
    {
        public static byte[,] s = new byte[,]
        {
            { 12, 4, 6, 2, 10, 5, 11, 9, 14, 8, 13, 7, 0, 3, 15, 1 },
            { 6, 8, 2, 3, 9, 10, 5, 12, 1, 14, 4, 7, 11, 13, 0, 15 },
            { 11, 3, 5, 8, 2, 15, 10, 13, 14, 1, 7, 4, 12, 9, 6, 0 },
            { 12, 8, 2, 1, 13, 4, 15, 6, 7, 0, 10, 5, 3, 14, 9, 11 },
            { 7, 15, 5, 10, 8, 1, 6, 13, 0, 9, 3, 14, 11, 4, 2, 12 },
            { 5, 13, 15, 6, 9, 2, 12, 10, 11, 7, 8, 1, 4, 3, 14, 0 },
            { 8, 14, 2, 5, 6, 9, 1, 12, 15, 4, 11, 0, 13, 10, 3, 7 },
            { 1, 7, 14, 13, 0, 5, 8, 3, 4, 15, 10, 6, 9, 12, 11, 2 }
        };

        #region t
        static uint Get4BitByIndex(uint block, int index)
        {
            return block << 4 * (7 - index) >> 4 * 7;
        }

        static uint method_t(uint subBlock)
        {
            uint encrypted = 0;

            for (int i = 0; i < 8; i++)
                encrypted += Convert.ToUInt32((uint)s[i, Get4BitByIndex(subBlock, i)] << 4 * i);

            return encrypted;
        }
        #endregion

        #region g[k]
        static uint CycleShift(uint value, int bits)
        {
            return (value << bits) + (value >> sizeof(uint) * 8 - bits);
        }

        static uint method_gk(uint key, uint subBlock)
        {
            return CycleShift(method_t(subBlock + key), 11);
        }
        #endregion

        #region Развёртывание ключа
        static List<uint> key_unpack(string key)
        {
            uint[] keys = new uint[32];

            for (int i = 0; i < 8; i++)
            {
                keys[i] = Convert.ToUInt32(key.Substring(i * 8, 8), 16);
                keys[i + 8] = Convert.ToUInt32(key.Substring(i * 8, 8), 16);
                keys[i + 16] = Convert.ToUInt32(key.Substring(i * 8, 8), 16);
                keys[i + 24] = Convert.ToUInt32(key.Substring((7 - i) * 8, 8), 16);
            }

            return keys.ToList();
        }
        #endregion

        #region G[k] and G*[k]
        static void method_Gk(uint key, ref uint a1, ref uint a0)
        {
            uint a0_old = a0;

            Console.WriteLine("\n" + Convert.ToString(a0, 16) + "->" + Convert.ToString(method_gk(key, a0), 16));

            a0 = method_gk(key, a0) ^ a1;

            Console.WriteLine("Final a0: " + Convert.ToString(a0, 16));

            a1 = a0_old;
        }

        static uint method_Gfinish(uint key, uint a1, uint a0)
        {
            return (method_gk(key, a0) ^ a1);
        }
        #endregion

        static void Main(string[] args)
        {
            ulong block = Convert.ToUInt64("fedcba9876543210", 16);
            uint a1 = Convert.ToUInt32(block >> 32);
            uint a0 = Convert.ToUInt32(block << 32 >> 32);

            string key = "ffeeddccbbaa99887766554433221100f0f1f2f3f4f5f6f7f8f9fafbfcfdfeff";

            List<uint> keys = key_unpack(key);
            
            //шифрование
            for (int i = 0; i < 32; i++)
            {
                method_Gk(keys[i], ref a1, ref a0);
            }

            //Console.WriteLine(Convert.ToString(method_Gfinish(keys[31], a1, a0), 16) + Convert.ToString(a0, 16));

            
            //расшифрование
            a1 = method_Gfinish(keys[30], a1, a0);
            //a0 = a0;

            keys = key_unpack(key);

            for (int i = 31; i > 0; i--)
            {
                method_Gk(keys[i], ref a1, ref a0);
            }

            Console.WriteLine(Convert.ToString(method_Gfinish(keys[0], a1, a0), 16) + Convert.ToString(a0, 16));
            
    
            Console.ReadLine();
        }
    }
}
