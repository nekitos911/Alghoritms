using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaffmanCode
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
                    var file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    var reader = new StreamReader(file, Encoding.Default);
                    var inputString = reader.ReadToEnd();
                    file.Close();
                    reader.Close();
                    encode(inputString,new FileInfo(fileName).Name);

                    break;
                case "-d":
                    decode(new FileInfo(fileName).Name);
                    break;
                default:
                    Error("Using:\n -e fileName for enc / -d Enc fileName for dec");
                    break;
            }
            Console.ReadKey();
        }

        static void encode(string input,string fileName)
        {
            var haffman = new Tree(input);
            var bits = haffman.Encode();
            var treeBits = haffman.getEncodedTree();
            var writer = new BinStream(fileName + ".enc");
            writer.WriteBits(treeBits,FileMode.Create);
            writer.WriteBits(new BitArray(Encoding.GetEncoding(1251).GetBytes("\n\0\n")),FileMode.Append);
            writer.WriteBits(bits,FileMode.Append);
        }

        static void decode(string fileName)
        {
            var haffman = new Tree(fileName);
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    using (var binReader = new BinaryReader(file))
                    {
                        var treeBytes = new List<byte>();
                        var tmp = new byte[3];
                        //Check for stop bytes "\n\0\n"
                        while ((tmp[0] = binReader.ReadByte()) != (byte)'\n' & (tmp[1] = binReader.ReadByte()) != (byte)'\0' & (tmp[2] = binReader.ReadByte()) != (byte)'\n')
                        {
                            treeBytes.AddRange(tmp);
                        }
                        var count = treeBytes.Count;
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        var treeBits = new BinStream().ReadBits(treeBytes.ToArray());
                        reader.BaseStream.Seek(count + 4, SeekOrigin.Begin);
                        var fileBits = new BinStream().ReadBits(binReader.ReadBytes((int)file.Length - count - 4));
                        File.WriteAllText("DEC" + fileName.Remove(fileName.LastIndexOf(".")), haffman.Decode(treeBits, fileBits),
                            Encoding.GetEncoding(1251));
                    }
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
