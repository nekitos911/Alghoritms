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
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Error("Using:\n -e fileName for enc / -d TreeName for dec");
            }
            var fileName = args[1];
            switch (args[0])
            {
                case "-e":
                    using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(file, Encoding.GetEncoding(1251)))
                        {
                            var inputString = reader.ReadToEnd();
                            encode(inputString, new FileInfo(fileName).Name + ".enc");
                            Console.WriteLine("Done, file saved as " + new FileInfo(fileName).Name + ".enc");
                        }
                    }
                    break;
                case "-d":
                    decode(new FileInfo(fileName).Name);
                    Console.WriteLine("Done");
                    break;
                default:
                    Error("Using:\n -e fileName for enc / -d Enc fileName for dec");
                    break;
            }
        }

        static void encode(string input, string fileName)
        {
            using (var file = new FileStream(fileName, FileMode.Create))
            {

            }

            var coder = new Coder();
           // input = input.Replace("\r","");
            var encodeStr = new StringBuilder(coder.Encode(input + "\n\0\n", fileName));
            var builder = new StringBuilder();
            foreach (var freq in coder.getFrequencies())
            {
                builder.Append(freq.Key);
                builder.Append(" ");
                builder.Append(freq.Value);
                builder.Append("\n");

            }

            builder.Append("\n\0\n");

            var writer = new BinStream(fileName);
            writer.WriteBits(new BitArray(Encoding.GetEncoding(1251).GetBytes(builder.ToString())), FileMode.Append);

            OutputBits(new BinStream(fileName), encodeStr.ToString(), fileName);

        }

        private static void OutputBits(BinStream stream, string toWrite, string fileName)
        {
            var bytes = BigInteger.Parse(toWrite).ToByteArray();
            var bitsStr = string.Concat(bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).Reverse());

            var list = new List<bool>(bitsStr.Length);
            foreach (var c in bitsStr)
            {
                list.Add(c != '0');
            }

            var bits = new BitArray(list.ToArray());
            stream.WriteBits(bits, FileMode.Append);
        }

        static void decode(string fileName)
        {
            var coder = new Coder();

            BitArray fileBits;
            var tree = new StringBuilder();
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                using (var reader = new BinaryReader(file))
                {

                    reader.BaseStream.Seek(0, SeekOrigin.Begin);
                    var tmp = new char[3];
                    while (true)
                    {
                        if ((tmp[0] = Encoding.GetEncoding(1251).GetString(new byte[1] {reader.ReadByte()})[0]) == '\n'
                            & (tmp[1] = Encoding.GetEncoding(1251).GetString(new byte[1] {reader.ReadByte()})[0]) ==
                            '\0'
                            & (tmp[2] = Encoding.GetEncoding(1251).GetString(new byte[1] {reader.ReadByte()})[0]) ==
                            '\n')
                            break;
                        reader.BaseStream.Seek(-3, SeekOrigin.Current);
                        while ((tmp[0] = Encoding.GetEncoding(1251).GetString(new byte[1] {reader.ReadByte()})[0]) !=
                               ' ')
                        {
                                tree.Append((char) tmp[0]);
                        }
                        if ((tmp[1] = Encoding.GetEncoding(1251).GetString(new byte[1] { reader.ReadByte() })[0]) ==
                            ' ')
                            tree.Append((char)tmp[0]);
                        reader.BaseStream.Seek(-1, SeekOrigin.Current);

                        while ((tmp[0] = Encoding.GetEncoding(1251).GetString(new byte[1] {reader.ReadByte()})[0]) !=
                               '\n')
                        {
                            tree.Append((char) tmp[0]);
                        }

                        tree.Append('\n');
                    }

                    reader.BaseStream.Seek(1, SeekOrigin.Current);
                    fileBits = new BinStream().ReadBits((reader.ReadBytes((int) (file.Length - reader.BaseStream.Position))));
                    var dict = new Dictionary<char, decimal>();
                    for (int i = 0; i < tree.Length - 1; i++)
                    {
                        var num = new StringBuilder();
                        var c = tree[i];
                        i++;
                        char n;
                        while ((n = tree[i]) != '\n')
                        {
                            num.Append(n);
                            i++;
                        }
                       // if(c != '\r')
                            dict.Add(c, int.Parse(num.ToString()));

                    }

                    var builder = new StringBuilder();
                    for (int i = 0; i < fileBits.Length; i++)
                    {
                        builder.Append(fileBits.Get(i) ? '1' : '0');
                    }

                    var encoded = FromBinary(builder.ToString()).ToString();
                     var decoded = coder.Decode(encoded.ToString(), dict);
                     File.WriteAllText("DEC" + fileName.Remove(fileName.LastIndexOf(".")),
                         decoded.Remove(decoded.Length - 2),
                         Encoding.GetEncoding(1251));
                }
            }
        }


        static BigInteger FromBinary(string input)
        {
            BigInteger big = new BigInteger();
            var count = input.Length;
            var index = 1.0D;

            foreach (var c in input)
            {
                big <<= 1;
                big += new BigInteger(c - '0');
                Util.ShowPercents(count,ref index);
            }

            return big;
        }

        static void Error(string err)
        {
            Console.WriteLine(err);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
