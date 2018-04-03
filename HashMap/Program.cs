using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab4
{
    class Program
    {
        static void Main(string[] args)
        {
            HashMap<int,int> map = new HashMap<int, int>();
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < 10; i++)
            {
                map.Add(i,i);
            }

            map.Add(1, 3);
            foreach (var e in map)
            {
                Console.WriteLine("key: " + e.Key + " value: " + e.Value);
            }

            Console.ReadKey();

        }
    }
}
