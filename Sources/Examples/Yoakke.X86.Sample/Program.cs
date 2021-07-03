using System;
using System.IO;
using Yoakke.X86.Instructions;
using Yoakke.X86.Operands;
using Yoakke.X86.Writers;

namespace Yoakke.X86.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new StringWriter();
            var writer = new IntelAssemblyWriter(sw);
            writer.Write(new Add(
                Registers.Eax,
                new Indirect(DataWidth.Dword, new Address(Registers.Ecx, new ScaledIndex(Registers.Edx, 4), 23))));
            Console.WriteLine(sw);
        }
    }
}
