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
            var writer = new AssemblyWriter();
            writer.Write(new Add(Registers.Eax, Registers.Ecx)).Write(' ').WriteComment("Hello\nWorld");
            Console.WriteLine(writer.Result);
        }
    }
}
