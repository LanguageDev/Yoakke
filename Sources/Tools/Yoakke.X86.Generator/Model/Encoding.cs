// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Yoakke.X86.Generator.Model
{
    /// <summary>
    /// A description on how to encode an <see cref="InstructionForm"/>.
    /// </summary>
    public class Encoding : IXmlSerializable
    {
        // TODO: This is not correct. The opcode can be followed by a ModRM and then another Opcode
        // So we should only consume consecutive tags here, but I didn't figure out how to do that yet

        /// <summary>
        /// The main opcodes of the instruction.
        /// </summary>
        [XmlElement("Opcode")]
        public List<Opcode> Opcodes { get; set; } = new();

        /// <summary>
        /// The postbyte some instruction encodings require.
        /// </summary>
        [XmlElement("Opcode")]
        public Opcode? Postbyte { get; set; }

        /// <inheritdoc/>
        public XmlSchema? GetSchema() => null;

        /// <inheritdoc/>
        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var subtree = reader.ReadSubtree();
            XDocument xDoc = XDocument.Load(subtree);

            var pastOpcode = false;
            foreach (var element in xDoc.Element("Encoding")!.Elements())
            {
                if (element.Name == "Opcode")
                {
                    var opcode = Deserialize<Opcode>(element.CreateReader());
                    if (pastOpcode)
                    {
                        Debug.Assert(this.Postbyte is null, "There can only be a single postbyte");
                        this.Postbyte = opcode;
                    }
                    else
                    {
                        this.Opcodes.Add(opcode);
                    }
                }
                else
                {
                    if (this.Opcodes.Count > 0) pastOpcode = true;
                }
            }

            subtree.Close();
            reader.ReadEndElement();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer) => throw new NotImplementedException();

        private static T Deserialize<T>(XmlReader reader) =>
            (T?)new XmlSerializer(typeof(T)).Deserialize(reader) ?? throw new InvalidOperationException();
    }
}
