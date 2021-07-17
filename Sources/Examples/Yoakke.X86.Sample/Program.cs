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
            var bs = new byte[]
            {
                0x55, 0x89, 0xE5, 0x83, 0xEC, 0x10, 0xC7, 0x45, 0xFC, 0x00, 0x00, 0x00, 0x00,
                0xC7, 0x45, 0xF8, 0x00, 0x00, 0x00, 0x00, 0xEB, 0x0A, 0x8B, 0x45, 0xF8, 0x01,
                0x45, 0xFC, 0x83, 0x45, 0xF8, 0x01, 0x83, 0x7D, 0xF8, 0x09, 0x7E, 0xF0, 0x8B,
                0x45, 0xFC, 0xC9, 0xC3
            };
            var reader = new AssemblyBinaryReader(new BinaryReader(new MemoryStream(bs)));

            var builder = new AssemblyBuilder();

            for (var i = 0; i < 14; ++i)
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
