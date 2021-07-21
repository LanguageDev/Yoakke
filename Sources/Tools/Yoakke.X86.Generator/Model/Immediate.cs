// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Yoakke.X86.Generator.Model
{
    /// <summary>
    /// Encodes an immediate operand.
    /// </summary>
    public class Immediate
    {
        /// <summary>
        /// The size of the <see cref="Immediate"/> value in bytes.
        /// </summary>
        [XmlIgnore]
        public int Size { get; set; }

        /// <summary>
        /// The operand number this <see cref="Immediate"/> belongs to.
        /// </summary>
        [XmlIgnore]
        public int OperandNumber { get; set; }

        /// <summary>
        /// The string-encoded size in the XML.
        /// </summary>
        [XmlAttribute(AttributeName = "size")]
        public string SizeString
        {
            get => this.Size.ToString();
            set => this.Size = int.Parse(value);
        }

        /// <summary>
        /// The operand number encoded with a '#'.
        /// </summary>
        [XmlAttribute(AttributeName = "value")]
        public string OperandNumberString
        {
            get => $"#{this.OperandNumber}";
            set => this.OperandNumber = int.Parse(value[1..]);
        }
    }
}
