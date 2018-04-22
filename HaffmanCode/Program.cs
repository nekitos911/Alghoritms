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
                    Encode(inputString,new FileInfo(fileName).Name);
                    Console.WriteLine("Done, file saved as " + new FileInfo(fileName).Name + ".enc");
                    break;
                case "-d":
                    Decode(new FileInfo(fileName).Name);
                    Console.WriteLine("Done");
                    break;
                default:
                    Error("Using:\n -e fileName for enc / -d Enc fileName for dec");
                    break;
            }
            Console.ReadKey();
        }

        static void Encode(string input,string fileName)
        {
            var haffman = new Tree(input);
            var bits = haffman.Encode();
            var treeBits = haffman.getEncodedTree();
            var writer = new BinStream(fileName + ".enc");
            writer.WriteBits(treeBits,FileMode.Create);
            writer.WriteBits(new BitArray(Encoding.GetEncoding(1251).GetBytes("\n\0\n")),FileMode.Append);
            writer.WriteBits(bits,FileMode.Append);
        }

        static void Decode(string fileName)
        {
            var haffman = new Tree(fileName);
            using (var file = new FileStream(fileName, FileMode.Open))
            {
                    using (var binReader = new BinaryReader(file))
                    {
                        var treeBytes = new List<byte>();
                        var tmp = new byte[3];
						//Check for stop bytes "\n\0\n"
                        while (true)
                        {
                            tmp[0] = binReader.ReadByte();
                            if ((tmp[0]) == (byte) '\n')
                            {
                                tmp[1] = binReader.ReadByte();
                                if ((tmp[1]) == (byte) '\0')
                                {
                                    tmp[2] = binReader.ReadByte();
                                    if ((tmp[2]) == (byte) '\n')
                                    {
                                        break;
                                    }
                                        treeBytes.AddRange(tmp);
                                }
                                else
                                {
                                    treeBytes.Add(tmp[0]);
                                    treeBytes.Add(tmp[1]);
                                }
                                
                            }
                            else
                            {
                                treeBytes.Add(tmp[0]);
                            }
                        }
                        var count = treeBytes.Count;
                        file.Seek(0, SeekOrigin.Begin);
                        var treeBits = new BinStream().ReadBits(treeBytes.ToArray());
                        file.Seek(count + 4, SeekOrigin.Begin);
                        var fileBits = new BinStream().ReadBits(binReader.ReadBytes((int)file.Length - count - 4));
                        File.WriteAllText("DEC" + fileName.Remove(fileName.LastIndexOf(".")), haffman.Decode(treeBits, fileBits),
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
