using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Yoakke.X86.Generator.Model;

namespace Yoakke.X86.Generator
{
    /*
     * Consumes the format found on https://github.com/Maratyszcza/Opcodes
     */

    internal static class Program
    {
        private static void Main(string[] args)
        {
            var serializer = new XmlSerializer(typeof(InstructionSet));
            var isa = (InstructionSet?)serializer.Deserialize(new FileStream(@"c:\TMP\x86_gen\Opcodes\opcodes\x86.xml", FileMode.Open, FileAccess.Read));
            if (isa is null) throw new InvalidOperationException();

            var a = isa.Instructions.First(i => i.Forms.Any(f => f.Operands.Any(o => o.IsInput && o.IsOutput)));

            var supported = 0;
            var unsupported = 0;

            var result = new StringBuilder();
            foreach (var instruction in isa.Instructions)
            {
                try
                {
                    var source = ClassGenerator.GenerateInstruction(instruction);
                    result.AppendLine(source);
                    ++supported;
                }
                catch (NotSupportedException)
                {
                    // Console.WriteLine($"Can't support {instruction.Name}");
                    ++unsupported;
                }
            }

            Console.WriteLine(result);
            Console.WriteLine($"supported: {supported}, unsupported: {unsupported}");
        }
    }
}
