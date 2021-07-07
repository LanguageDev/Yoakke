using System;
using System.IO;
using Yoakke.X86.Operands;
using Yoakke.X86.Writers;

namespace Yoakke.X86.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new AssemblyBuilder()
                .Label("start")
                .Add(
                    new Indirect(DataWidth.Dword, new Address(Registers.Ss, Registers.Ecx, new ScaledIndex(Registers.Edx, 4), 23)),
                    Registers.Eax,
                    "just some test\nthis is the next line");
            var assembly = builder.Assembly;
            var context = new AssemblyContext { AddressSize = DataWidth.Dword };
            assembly.Validate(context);

            var writer = new AssemblyWriter();
            writer.Settings = new AssemblyWriterSettings
            {
                SyntaxFlavor = SyntaxFlavor.Intel,
            };
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);
        }
    }
}
