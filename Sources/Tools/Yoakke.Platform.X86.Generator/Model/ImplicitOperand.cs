// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Xml.Serialization;

namespace Yoakke.Platform.X86.Generator.Model
{
    /// <summary>
    /// An implicit operand to an <see cref="InstructionForm"/>.
    /// They are not typed into the assembly code for the user, but they use it anyway, causing side-effects on write.
    /// </summary>
    public class ImplicitOperand
    {
        /// <summary>
        /// The operand type.
        /// </summary>
        [XmlAttribute(AttributeName = "id")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// True, if this is an input operand.
        /// </summary>
        [XmlAttribute(AttributeName = "input")]
        public bool IsInput { get; set; }

        /// <summary>
        /// True, if this is an output operand.
        /// </summary>
        [XmlAttribute(AttributeName = "output")]
        public bool IsOutput { get; set; }
    }
}
