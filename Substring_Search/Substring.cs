using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab5
{
    class Substring
    {
        public static class BoyerMoore
        {
            private static Dictionary<char, int> tableshift = new Dictionary<char, int>();

            private static void TableShift(string str)
            {
                for (int i = 0; i < char.MaxValue; i++)
                {
                    tableshift.Add((char)i, str.Length);
                }

                for (int i = 0; i < str.Length - 1; i++)
                {
                    tableshift[str[i]] = str.Length - i - 1;
                }
             

            }

            public static List<int> GetSub(string input,string substring) 
            {
                TableShift(substring);
                var res = new List<int>();
                var i = substring.Length - 1;
                var j = i;
                var k = i;
                while (i < input.Length)
                {
                    j = substring.Length - 1;
                    k = i;
                    while (j >= 0 && substring[j] == input[k])
                    {
                        k--;
                        j--;
                    }

                    if (j + 1 == 0 && substring[j + 1] == input[k + 1])
                    {
                        res.Add(k + 1);
                        i += substring.Length;
                    }
                    else
                    {
                        i += tableshift[input[i]];
                    }
                }           
                if(res.Count == 0)
                    res.Add(-1);
                return res;
            }
        }


        public static class RabinKarp
        {
            private const int P = 31; //Простое число
            private const int R = int.MaxValue;
            private static int Hash(string x)
            {
                return x.Select((t, i) => ((int) Math.Pow(P, x.Length - 1 - i) * t) % R).Sum();
            }

            public static List<int> GetSub(string input, string substring)
            {
                var res = new List<int>();
                var n = input.Length;
                var m = substring.Length;
                var hashIn = Hash(input.Substring(0,m));
                var hashSub = Hash(substring);
                for (var i = 0; i <= n - m; i++)
                {
                    if (hashIn == hashSub)
                    {
                        res.Add(i);
                    }
                    if(i == (n - m)) break;
                        hashIn = Hash(input.Substring(i + 1,m)) % R;
                }

                return res;
            }

        }

        public static class KMP
        {
            private static List<int> PrefixFunc(string s)
            {
                var n = s.Length;
                var pi = new List<int>();
                for (var i = 0; i < n; i++)
                {
                    pi.Add(0);
                }
                for (var i = 1; i < n; i++)
                {
                    var j = pi[i - 1];
                    while (j > 0 && s[i] != s[j])
                    {
                        j = pi[j - 1];
                    }
                    if (s[i] == s[j]) ++j;
                    pi[i] = j;
                }
                return pi;
            }

            public static List<int> GetSub(string input, string substring)
            {
                var res = new List<int>();
                var f = PrefixFunc(substring);
                var k = 0;
                for (var i = 0; i < input.Length; i++)
                {
                    while (k > 0 && substring[k] != input[i])
                    {
                        k = f[k - 1];
                    }

                    if (substring[k] == input[i])
                    {
                        k++;
                    }

                    if (k == substring.Length)
                    {
                        res.Add(i - substring.Length + 1);
                        k = 0;
                    }
                }
                return res;
            }
        }

    }
}