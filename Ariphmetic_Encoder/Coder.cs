using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace AriphmeticEncoder
{
    class Coder
    {
        public Coder()
        {
            low = 0;
            high = 9999999999999999999;
            codes = new List<Character>();
        }

        private ulong low;
        private ulong high;
        private int length;

        private List<Character> codes;
        private Dictionary<char, decimal> frequencies;

        private void InitializeEncoder(string input)
        {
            frequencies = new Dictionary<char,decimal>(256);
            foreach (var c in input)
            {
                if (!frequencies.ContainsKey(c))
                {
                    frequencies.Add(c, 0);
                }

                frequencies[c]++;
            }
            frequencies = frequencies.Reverse().ToDictionary(pair => pair.Key, pair => pair.Value); 
        }

       

        public string Encode(string input,string fileName)
        {
            length = input.Length;

            InitializeEncoder(input);

            InitializeTable();

            var encoded = new StringBuilder();

            var index = 1.0D;

            foreach (var c in input)
            {
                var symb = CharToCharacter(c);
                EncodeSymbol(symb,ref encoded,fileName);
                Util.ShowPercents(length,ref index);
            }

            encoded.Append(Trim(high, low));
            return encoded.ToString();
            // return new BitArray(Encoding.GetEncoding(1251).GetBytes(encoded.ToString()));
        }

        private void EncodeSymbol(Character c,ref StringBuilder encode,string fileName)
        {
            var range = Convert.ToUInt64(high - low + 1); 

            high = Convert.ToUInt64(low + range * c.High - 1);                                                     
            low = Convert.ToUInt64(low + range * c.Low);

            var highStr = new StringBuilder().Append('0', 19 - Convert.ToString(high).Length) + Convert.ToString(high);
            var lowStr = new StringBuilder().Append('0', 19 - Convert.ToString(low).Length) + Convert.ToString(low);

            /*if ((high + 1)  == low )
             {
                 encode.Append(lowStr);

                low = 0;
                high = 9999999999999999999;
                range = Convert.ToUInt64(high - low + 1);
                high = Convert.ToUInt64(low + range * c.High - 1);
                low = Convert.ToUInt64(low + range * c.Low);
            }*/


            while (lowStr[0] == highStr[0])
            {
                encode.Append(lowStr[0]);
                high = Convert.ToUInt64(highStr.Substring(1) + "9");
                low = Convert.ToUInt64(lowStr.Substring(1) + "0");
                highStr = new StringBuilder().Append('0', 19 - Convert.ToString(high).Length) + Convert.ToString(high);
                lowStr = new StringBuilder().Append('0', 19 - Convert.ToString(low).Length) + Convert.ToString(low);
            }
           
        }

        
        private string Trim(ulong high, ulong low)
        {
            var encoded = new StringBuilder();
            while (high % 10 == 0)
            {
                high /= 10;
            }

            while (low % 10 == 0)
            {
                low /= 10;
            }

            var temp = high.ToString().Length - low.ToString().Length;

            if (temp > 0)
            {
                while (high.ToString().Length != low.ToString().Length)
                {
                    high /= 10;
                }

                if (high == low)
                {
                    encoded.Append((low).ToString());
                }
                else
                {
                    encoded.Append((high - 1).ToString());
                }
               
            }
            else if (temp < 0)
            {
                while (high.ToString().Length != low.ToString().Length)
                {
                    low /= 10;
                }
                if (high == low)
                {
                    encoded.Append((high - 1).ToString());
                }
                encoded.Append((low + 1).ToString());
            }
            else
            {
                encoded.Append(low.ToString());
            }

            while (encoded.ToString().Last() == '0')
            {
                encoded.Remove(encoded.Length - 1, 1);
            }
            return encoded.ToString();
        }

        private Character CharToCharacter(char c)
        {
            return codes.SingleOrDefault(s => s.Symbol == c);
        }

        private void InitializeTable()
        {
            var count = 0.0M;
            foreach (var f in frequencies)
            {
                count += f.Value;
            }
            var codesDict = frequencies;      

            var counter = 0.0M;
            foreach (var code in codesDict)
            {
                codes.Add(new Character(code.Key,decimal.Round(counter / count,20),decimal.Round((counter + code.Value) / count,20)));
                counter +=  code.Value;
            }
           
        }

        public Dictionary<char, decimal> getFrequencies()
        {
            return frequencies;
        }

        public string Decode(string bits,Dictionary<char,decimal> frequencies)
        {
            codes = new List<Character>();
            this.frequencies = frequencies;

            InitializeTable();
            low = 0;
            high = 9999999999999999999;
            var decoded = new StringBuilder(bits);
            var count = decoded.Length;
      
            if (decoded.Length % 15 != 0)
            {
                decoded.Append('0', 15 - decoded.Length % 15);
            }
            var index = 1.0D;
            //decoded.Append('0',16);
            var res = new StringBuilder();
            var c = '0';
            while (true)
            {
                if (decoded.Length < 15)
                    break;
                c = DecodeSymbol(ref decoded,ref res);
                res.Append(c);

                if(res.Length > 3 && res.ToString()[res.Length - 1] == '\n' && res.ToString()[res.Length - 2] == '\0' && res.ToString()[res.Length - 3] == '\n')
                    break;
                Util.ShowPercents(count,ref index);
            }

            return res.ToString();
        }

        private char DecodeSymbol(ref StringBuilder decode,ref StringBuilder res)
        {
            var code = (ulong)0;
            try
            {
               code = Convert.ToUInt64(decode.ToString().Substring(0, 19));
            }
            catch (Exception ex)
            {
                code = Convert.ToUInt64(code.ToString() + new StringBuilder().Append('0', 19 - code.ToString().Length));
                //Console.ReadKey();
            }


            var range = Convert.ToUInt64(high - low + 1);

            var oldLow = low;
            var oldHigh = high;

            var index = decimal.Round((decimal) (code - low) / range, 19);

            var symb = index < 1 ? codes.SingleOrDefault(s => (s.Low <= index & index < s.High)) : codes.SingleOrDefault(s => s.High == 1);

            high = Convert.ToUInt64(low + range * symb.High - 1);
            low = Convert.ToUInt64(low + range * symb.Low);

           /* if ((high + 1) / 10 == low / 10)
            {
                low = 0;
                high = 9999999999999999999;
                decode.Remove(0, 19);
                if (decode.Length <= 19)
                {
                    if (Convert.ToUInt64(decode.ToString().Substring(0, decode.Length)) == 0)
                        return symb.Symbol;
                }
                code = Convert.ToUInt64(decode.ToString().Substring(0, 19));
                range = Convert.ToUInt64(high - low + 1);
                index = decimal.Round((decimal)(code - low) / range, 15);

                symb = index < 1 ? codes.SingleOrDefault(s => (s.Low <= index & index < s.High)) : codes.SingleOrDefault(s => s.High == 1);
                high = Convert.ToUInt64(low + range * symb.High - 1);
                low = Convert.ToUInt64(low + range * symb.Low);
            }*/

            var highStr = new StringBuilder().Append('0', 19 - Convert.ToString(high).Length) + Convert.ToString(high);
            var lowStr = new StringBuilder().Append('0', 19 - Convert.ToString(low).Length) + Convert.ToString(low);


            if (lowStr[0] == highStr[0])
            {
                while (lowStr[0] == highStr[0])
                {
                    decode.Remove(0, 1);
                    high = Convert.ToUInt64(highStr.Substring(1) + "9");
                    low = Convert.ToUInt64(lowStr.Substring(1) + "0");
                    highStr = new StringBuilder().Append('0', 19 - Convert.ToString(high).Length) +
                              Convert.ToString(high);
                    lowStr = new StringBuilder().Append('0', 19 - Convert.ToString(low).Length) + Convert.ToString(low);
                }
            }

            return symb.Symbol;
        }
    }
}
