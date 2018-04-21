using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaffmanCode
{
    class BinStream
    {
        private readonly string inputFile;

        public BinStream(string inputFile)
        {
            this.inputFile = inputFile;
        }

        public BinStream()
        {
        }

        public void WriteBits(BitArray bits, FileMode fileMode)
        {
            if(inputFile.Length == 0) throw new ArgumentException();
            var newBits = new BitArray(bits.Length + (8 - bits.Length % 8));
            newBits.SetAll(true);
            for (int i = 0; i < bits.Length; i++)
            {
                newBits.Set(i, bits.Get(i));
            }
            var res = new byte[newBits.Length / 8];
            if (bits.Length % 8 != 0)
            {
                res = new byte[newBits.Length / 8 + 1];
                var count = 0;
                for (int i = bits.Length; i < newBits.Length; i++)
                {
                    newBits.Set(i, false);
                    count++;
                }
                var b = new BitArray(8);
                b.SetAll(true);
                for (int i = b.Count - 1; i >= b.Count - count; i--)
                {
                    b.Set(i, false);
                }
                b.CopyTo(res, res.Length - 1);
            }
            newBits.CopyTo(res, 0);
            using (var file = new FileStream(inputFile, fileMode))
            {
                using (var writer = new BinaryWriter(file))
                {
                    writer.Write(res);
                }
            }
        }

        public BitArray ReadBits(byte[] bytes)
        {
            var inputBits = new BitArray(bytes);
            var count = 0;
            for (int i = inputBits.Count - 8; i < inputBits.Count; i++)
            {
                if (!inputBits.Get(i))
                {
                    count++;
                }
            }
            var res = new BitArray((int)(inputBits.Count - count - 8));
            for (int i = 0; i < res.Length; i++)
            {
                res.Set(i, inputBits.Get(i));
            }
            return res;
        }
    }
}
