using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Lab2
{
    class Program
    {
        public static Random rnd = new Random(DateTime.Now.Millisecond);
        public static string RndString()
        {
            var str = "abcdefghajklmnopqrstuvwABCDEFGHIJKLMNOPQRSTUVW";
            var result = new StringBuilder();
            var size = 5;
            for (var i = 0; i < size; i++)
            {
                result.Append(str[rnd.Next(0, str.Length)]);
            }

            return result.ToString();
        }
        static void Main(string[] args)
        {
            var queue = new PriorityQueue<int>();
           /* for (var i = 0; i < 10; i++)
            {
                var priority = (int)(rnd.NextDouble() * 100);
                var element = RndString();
                Console.WriteLine("Priority: " + priority + " Value: " + element);
                queue.Add(priority,element);
            }
            Console.WriteLine("Is empty: " + queue.IsEmpty());
            Console.WriteLine("Count: " + queue.Count);
            Console.WriteLine("Peek: " + queue.Peek());
            while (!queue.IsEmpty())
            {
                Console.WriteLine(queue.Remove() + " Removed");
            }

            Console.WriteLine("Is empty: " + queue.IsEmpty());
            Console.WriteLine("Count: " + queue.Count);*/
            queue.Add(6,3);
            queue.Add(5,2);
            queue.Add(9,10);
            queue.Add(7,6);
            queue.Add(8,8);
            queue.Add(4,1);
            while (!queue.IsEmpty())
            {
                Console.WriteLine(queue.RemoveMax());
            }
            Console.ReadKey();

        }
    }
}
