using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lab3
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new RBTreeTest(10000);

            test.StartTest();
          


            Console.ReadKey();
        }
    }
}
