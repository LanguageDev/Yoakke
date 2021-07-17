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
                .Mov(new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -4)), new Constant(0))
                .Mov(new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -8)), new Constant(0))
                .Jmp(LabelRef.Forward(".L2", out var l2))
                .Label(".L3", out var l3)
                .Mov(Registers.Eax, new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -8)))
                .Add(new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -4)), Registers.Eax)
                .Add(new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -8)), new Constant(1))
                .Label(l2)
                .Cmp(new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -8)), new Constant(9))
                .Jle(l3)
                .Mov(Registers.Eax, new Indirect(DataWidth.Dword, new Address(Registers.Ebp, -4)))
                .Leave()
                .Ret()
                ;
            var assembly = builder.Assembly;
            var context = new AssemblyContext { AddressSize = DataWidth.Dword };
            assembly.Validate(context);

            var writer = new AssemblyTextWriter
            {
                Settings = new AssemblyTextWriterSettings
                {
                    SyntaxFlavor = SyntaxFlavor.ATnT,
                    InstructionPadding = 5,
                }
            };
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);
        }
    }
}
