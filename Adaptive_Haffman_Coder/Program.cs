using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaptive_Haffman_Coder
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
                        using (var reader = new StreamReader(file, Encoding.Default))
                        {
                            var inputString = reader.ReadToEnd();
                            encode(inputString, new FileInfo(fileName).Name);
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
            var haffman = new Tree();
            var bits = haffman.Encode(input);
            var writer = new BinStream(fileName + ".enc");
            writer.WriteBits(bits, FileMode.Create);
        }

        static void decode(string fileName)
        {
            var haffman = new Tree();
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                    using (var binReader = new BinaryReader(file))
                    {
                        var fileBits = new BinStream().ReadBits(binReader.ReadBytes((int)file.Length));
                        File.WriteAllText("DEC" + fileName.Remove(fileName.LastIndexOf(".")), haffman.Decode(fileBits),
                            Encoding.GetEncoding(1251));
                    }
            }
        }

        static void Error(string err)
        {
            Console.WriteLine(err);
            Console.ReadKey();
            Environment.Exit(-1);
        }
    }
}
