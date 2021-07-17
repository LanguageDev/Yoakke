// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yoakke.X86.Generator.Model
{
    /// <summary>
    /// Represents a whole instruction set.
    /// </summary>
    public class InstructionSet
    {
        /// <summary>
        /// The name of the instruction set.
        /// </summary>
        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The <see cref="Instruction"/>s in this <see cref="InstructionSet"/>.
        /// </summary>
        [XmlElement("Instruction")]
        public List<Instruction> Instructions { get; set; } = new();

        /// <summary>
        /// Loads an <see cref="InstructionSet"/> from an XML file.
        /// </summary>
        /// <param name="filePath">The path to the XML.</param>
        /// <returns>The loaded <see cref="InstructionSet"/>.</returns>
        public static InstructionSet FromXmlFile(string filePath)
        {
            var serializer = new XmlSerializer(typeof(InstructionSet));
            var isa = (InstructionSet?)serializer.Deserialize(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            if (isa is null) throw new InvalidOperationException();
            isa.FixBackreferences();
            return isa;
        }

        private void FixBackreferences()
        {
            foreach (var instruction in this.Instructions)
            {
                foreach (var form in instruction.Forms)
                {
                    form.Instruction = instruction;
                    foreach (var encoding in form.Encodings)
                    {
                        encoding.Form = form;
                    }
                }
            }
        }
    }
}
