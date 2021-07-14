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
        /// <summary>
        /// The <see cref="Prefix"/>es belonging to this <see cref="Encoding"/>.
        /// </summary>
        [XmlElement("Prefix")]
        public List<Prefix> Prefixes { get; set; } = new();

        /// <summary>
        /// The main opcodes of the instruction.
        /// </summary>
        [XmlElement("Opcode")]
        public List<Opcode> Opcodes { get; set; } = new();

        /// <summary>
        /// The ModRM byte.
        /// </summary>
        [XmlElement("ModRM")]
        public ModRM? ModRM { get; set; }

        /// <summary>
        /// The postbyte some instruction encodings require.
        /// </summary>
        [XmlElement("Opcode")]
        public Opcode? Postbyte { get; set; }

        /// <summary>
        /// The immediates of the instruction.
        /// </summary>
        [XmlElement("Immediate")]
        public List<Immediate> Immediates { get; set; } = new();

        /// <summary>
        /// True, if the encoding had unsupported elements in the XML.
        /// </summary>
        [XmlIgnore]
        public bool HasUnsupportedElement { get; set; }

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
                    var opcode = Deserialize<Opcode>(element);
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

                    switch (element.Name.ToString())
                    {
                    case "Prefix":
                        this.Prefixes.Add(Deserialize<Prefix>(element));
                        break;

                    case "ModRM":
                        Debug.Assert(this.ModRM is null, "There can be only one ModR/M byte per encoding");
                        this.ModRM = Deserialize<ModRM>(element);
                        break;

                    case "Immediate":
                        this.Immediates.Add(Deserialize<Immediate>(element));
                        break;

                    default:
                        this.HasUnsupportedElement = true;
                        Console.WriteLine($"TODO: Unhandled {element.Name}");
                        break;
                    }
                }
            }

            subtree.Close();
            reader.ReadEndElement();
        }

        /// <inheritdoc/>
        public void WriteXml(XmlWriter writer) => throw new NotImplementedException();

        private static T Deserialize<T>(XElement element) => Deserialize<T>(element.CreateReader());

        private static T Deserialize<T>(XmlReader reader) =>
            (T?)new XmlSerializer(typeof(T)).Deserialize(reader) ?? throw new InvalidOperationException();
    }
}
