using System;
using Yoakke.X86.Operands;
using Yoakke.X86.Writers;

namespace Yoakke.X86.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new AssemblyBuilder()
                .Label("main")
                .Push(Registers.Ebp)
                .Mov(Registers.Ebp, Registers.Esp)
                .Sub(Registers.Esp, new Constant(16))
                .Mov(new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -4)), new Constant(0));
            var assembly = builder.Assembly;
            var context = new AssemblyContext { AddressSize = DataWidth.Dword };
            assembly.Validate(context);

            var writer = new AssemblyWriter();
            writer.Settings = new AssemblyWriterSettings
            {
                SyntaxFlavor = SyntaxFlavor.Intel,
                InstructionPadding = 5,
            };
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);
        }
    }
}
