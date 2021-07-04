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
            writer.SyntaxFlavor = SyntaxFlavor.Intel;
            // writer.SegmentSelectorInBrackets = true;
            // writer.InstructionsUpperCase = true;
            // writer.RegistersUpperCase = true;
            // writer.KeywordsUpperCase = true;
            writer.Write(
                new Add(
                    Registers.Eax,
                    new Indirect(DataWidth.Dword, new Address(Registers.Ss, Registers.Ecx, new ScaledIndex(Registers.Edx, 4), 23)))
                ).Write(' ').WriteComment("Hello\nWorld");
            Console.WriteLine(writer.Result);
        }
    }
}
