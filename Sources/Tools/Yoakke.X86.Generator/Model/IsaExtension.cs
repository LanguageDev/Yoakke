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
    /// The ISA extensions to x86.
    /// </summary>
    public enum IsaExtension
    {
        /// <summary>
        /// SSE1 extension.
        /// </summary>
        [XmlEnum(Name = "sse1")]
        Sse1,

        /// <summary>
        /// MMX extension.
        /// </summary>
        [XmlEnum(Name = "mmx")]
        Mmx,

        /// <summary>
        /// SSE2 extension.
        /// </summary>
        [XmlEnum(Name = "sse2")]
        Sse2,

        /// <summary>
        /// SSE3 extension.
        /// </summary>
        [XmlEnum(Name = "sse3")]
        Sse3,

        /// <summary>
        /// SSSE3 extension.
        /// </summary>
        [XmlEnum(Name = "ssse3")]
        Ssse3,

        /// <summary>
        /// VMX extension.
        /// </summary>
        [XmlEnum(Name = "vmx")]
        Vmx,

        /// <summary>
        /// SMX extension.
        /// </summary>
        [XmlEnum(Name = "smx")]
        Smx,

        /// <summary>
        /// SSE4.1 extension.
        /// </summary>
        [XmlEnum(Name = "sse41")]
        Sse4_1,

        /// <summary>
        /// SSE4.2 extension.
        /// </summary>
        [XmlEnum(Name = "sse42")]
        Sse4_2,
    }
}
