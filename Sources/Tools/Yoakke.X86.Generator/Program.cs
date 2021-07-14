using System;
using System.IO;
using System.Linq;
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

            Console.WriteLine($"{isa.Instructions.Count} instructions");
            Console.WriteLine($"{isa.Instructions.Select(i => i.Forms.Count).Sum()} forms");
            Console.WriteLine($"{isa.Instructions.Select(i => i.Forms.Where(f => f.Extension.Count == 0).Count()).Sum()} forms wo. extensions");

            foreach (var i in isa.Instructions.Where(i => i.Forms.Any(f => f.Extension.Count == 0)))
            {
                Console.WriteLine($"{i.Name}");
            }
        }
    }
}
