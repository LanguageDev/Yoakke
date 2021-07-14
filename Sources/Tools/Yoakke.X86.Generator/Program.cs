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

            var i = isa.Instructions.Where(i => i.Name == "PAVGUSB").First();
        }
    }
}
