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
                    Error("Using:\n -e fileName for enc / -d TreeName for dec");
                    break;
            }
            Console.ReadKey();
        }

        static void encode(string input,string fileName)
        {
            var haffman = new Tree(input,fileName + ".enc");
            var bits = haffman.Encode();
            var treeBits = haffman.getEncodedTree();
            /*var file = new FileStream(fileName + ".enc",FileMode.Create);
            var tree = new FileStream(fileName + ".enc.tree",FileMode.Create);
            var writer = new BinaryWriter(file);
            var fileBytes = WriteBits(bits);
            var treeBytes = WriteBits(treeBits);
            writer.Write(fileBytes); 
            writer.Close();
            file.Close();
            writer = new BinaryWriter(tree);
            writer.Write(treeBytes);
            writer.Close();
            tree.Close();*/
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
                        while ((tmp[0] = binReader.ReadByte()) != (byte)'\n' & (tmp[1] = binReader.ReadByte()) != (byte)'\0' & (tmp[2] = binReader.ReadByte()) != (byte)'\n')
                        {
                            treeBytes.AddRange(tmp);
                        }
                        var count = treeBytes.Count;
                        /*var str = "";
                        var str1 = "";
                        while ((str = reader.ReadLine()) != "\0" & (str1 = reader.ReadLine()) != "")
                        {
                            count += str.Length;
                            count += str1.Length;
                        }*/
                        reader.BaseStream.Seek(0, SeekOrigin.Begin);
                        var treeBits = new BinStream().ReadBits(treeBytes.ToArray());
                        reader.BaseStream.Seek(count + 4, SeekOrigin.Begin);
                        var fileBits = new BinStream().ReadBits(binReader.ReadBytes((int)file.Length - count - 4));
                        File.WriteAllText("DEC" + fileName.Remove(fileName.LastIndexOf(".")), haffman.Decode(treeBits, fileBits),
                            Encoding.GetEncoding(1251));
                    }
                }
            }
            /*var file = new FileStream(fileName + ".tree", FileMode.Open);
            var reader = new BinaryReader(file);
            var bytes = reader.ReadBytes((int) file.Length);
            var fileBits = new BitArray(bytes);
            var count = 0;
            for (int i = fileBits.Count - 8; i < fileBits.Count; i++)
            {
                if (!fileBits.Get(i))
                {
                    count++;
                }
            }

            var treeBits = new BitArray((int) (fileBits.Count - count - 8));
            for (int i = 0; i < treeBits.Length; i++)
            {
                treeBits.Set(i, fileBits.Get(i));
            }
            reader.Close();
            file.Close();
            file = new FileStream(fileName,FileMode.Open);
            reader = new BinaryReader(file);
            bytes = reader.ReadBytes((int)file.Length);
            fileBits = new BitArray(bytes);
            count = 0;
            for (int i = fileBits.Count - 8; i < fileBits.Count; i++)
            {
                if (!fileBits.Get(i))
                {
                    count++;
                }
            }
            var outFileBits = new BitArray((int)(fileBits.Count - count - 8));
            for (int i = 0; i < outFileBits.Length; i++)
            {
                outFileBits.Set(i, fileBits.Get(i));
            }

            var haffman = new Tree(fileName);
            File.WriteAllText("DEC" + fileName.Remove(fileName.LastIndexOf(".")), haffman.Decode(treeBits, outFileBits),
                Encoding.GetEncoding(1251));
            file.Close();
            reader.Close();
            //fs.Close();*/
        }

        static void Error(string err)
        {
            Console.WriteLine(err);
            Console.ReadKey();
            Environment.Exit(-1);
        }

        static byte[] WriteBits(BitArray bits)
        {
            var newBits = new BitArray(bits.Length + (8 - bits.Length % 8));
            newBits.SetAll(true);
            for (int i = 0; i < bits.Length; i++)
            {
                newBits.Set(i,bits.Get(i));
            }
            var res = new byte[newBits.Length / 8];
            if (bits.Length % 8 != 0)
            {
                res = new byte[newBits.Length / 8 + 1];
                var count = 0;
                for (int i = bits.Length; i < newBits.Length; i++)
                {
                    newBits.Set(i,false);
                    count++;
                }
                var b = new BitArray(8);
                b.SetAll(true);
                for (int i = b.Count - 1; i >= b.Count - count; i--)
                {
                    b.Set(i,false);
                }
                b.CopyTo(res,res.Length - 1);
            }
            newBits.CopyTo(res,0);
            return res;
        }
    }
}
