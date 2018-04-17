using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaffmanCode
{
    class Program
    {
        static void Main(string[] args)
        {
            var inputString = "beep boop beer!";

            var haffman = new Tree(inputString);
            var bits = haffman.Encode();
            Console.WriteLine("String in haffman: ");
            WriteBits(bits);
            Console.WriteLine(haffman.Decode(bits));
           
            Console.ReadKey();
        }

        static void WriteBits(BitArray bits)
        {
            var str = new StringBuilder();
            foreach (bool b in bits)
            {
                Console.Write(b ? 1 : 0);
            }

            Console.WriteLine();
        }

    }
}
