using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaffmanCode
{
    class Util
    {
        public static void ShowPercents(int count, ref double index)
        {
            if ((int)((index - 1) / count * 100) < (int)(index / count * 100))
            {
                Console.Clear();
                Console.WriteLine("{0} %", (int)(index / count * 100));
            }

            index++;
        }
    }
}
