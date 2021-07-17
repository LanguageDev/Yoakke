using System;
using System.IO;
using Yoakke.X86.Operands;
using Yoakke.X86.Readers;
using Yoakke.X86.Writers;

namespace Yoakke.X86.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var bs = new byte[] { 0x01, 0xC8, 0x50 };
            var reader = new AssemblyBinaryReader(new BinaryReader(new MemoryStream(bs)));

            var builder = new AssemblyBuilder();

            for (var i = 0; i < 3; ++i)
            {
                builder.Write(reader.ReadNext(out var len));
            }

            var assembly = builder.Assembly;
            var context = new AssemblyContext { AddressSize = DataWidth.Dword };

            var writer = new AssemblyTextWriter
            {
                Settings = new AssemblyTextWriterSettings
                {
                    SyntaxFlavor = SyntaxFlavor.Intel,
                    InstructionPadding = 5,
                }
            };
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);
        }
    }
}
