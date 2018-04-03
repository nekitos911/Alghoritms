using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab5
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Рабин - Карп: ");
            Console.WriteLine();
            var rabinKarp = Substring.RabinKarp.GetSub("Helloaloslolo", "lo");
            foreach (var e in rabinKarp)
            {
                Console.Write(e + " ");
            }
            Console.WriteLine();
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Кнут - Моррис - Пратт: ");
            Console.WriteLine();
            var m = Substring.KMP.GetSub("Helloaloslolo","lo");

            foreach (var e in m)
            {
                Console.Write(e + " ");
            }
            Console.WriteLine();
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Бойер - Мур: ");
            Console.WriteLine();
            var b = Substring.BoyerMoore.GetSub("Helloaloslolo", "lo");

            foreach (var e in b)
            {
                Console.Write(e + " ");
            }
            Console.WriteLine();
            Console.WriteLine("-------------------------------");

            Console.ReadKey();
        }
    }
}
