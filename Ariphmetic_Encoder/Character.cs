using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AriphmeticEncoder
{
    class Character
    {
        public Character(char symbol, decimal low, decimal high)
        {
            Symbol = symbol;
            High = high;
            Low = low;
        }

        public char Symbol { get; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
    }
}
