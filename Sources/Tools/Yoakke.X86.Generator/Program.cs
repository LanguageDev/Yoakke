using System;
using System.Collections.Generic;
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
            var isa = InstructionSet.FromXmlFile(args[0]);

            var classes = ClassGenerator.GenerateIsaClasses(isa, out var withClasses);
            File.WriteAllText("Classes.cs", classes.ToString());

            var parser = ParserGenerator.Generate(isa, withClasses);
            File.WriteAllText("Parser.cs", parser);

            var supported = withClasses.Count;
            var unsupported = isa.Instructions.Count - supported;
            Console.WriteLine($"supported: {supported}, unsupported: {unsupported}");
        }
    }
}
