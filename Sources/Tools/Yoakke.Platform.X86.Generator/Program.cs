using System;
using System.IO;
using Yoakke.Platform.X86.Generator.Model;

namespace Yoakke.Platform.X86.Generator
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

            var writers = WriterGenerator.GenerateIsaWriters(withClasses);
            File.WriteAllText("Writers.cs", writers.ToString());

            var parser = ParserGenerator.Generate(isa, withClasses);
            File.WriteAllText("Parser.cs", parser);

            var supported = withClasses.Count;
            var unsupported = isa.Instructions.Count - supported;
            Console.WriteLine($"supported: {supported}, unsupported: {unsupported}");
        }
    }
}
