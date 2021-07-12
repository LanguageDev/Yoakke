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
    /// Different memory address types.
    /// </summary>
    public enum AddressType
    {
        /// <summary>
        /// Memory addressed by DS:EAX, or by rAX in 64-bit mode (only 0F01C8 MONITOR).
        /// </summary>
        [XmlEnum(Name = "BA")]
        BA,

        /// <summary>
        /// Memory addressed by DS:eBX+AL, or by rBX+AL in 64-bit mode (only XLAT).
        /// </summary>
        [XmlEnum(Name = "BB")]
        BB,

        /// <summary>
        /// Memory addressed by DS:eDI or by RDI (only 0FF7 MASKMOVQ and 660FF7 MASKMOVDQU).
        /// </summary>
        [XmlEnum(Name = "BD")]
        BD,

        /// <summary>
        /// Memory addressed by the DS:eSI or by RSI (only MOVS, CMPS, OUTS, and LODS). In 64-bit mode,
        /// only 64-bit (RSI) and 32-bit (ESI) address sizes are supported. In non-64-bit modes, only
        /// 32-bit (ESI) and 16-bit (SI) address sizes are supported.
        /// </summary>
        [XmlEnum(Name = "X")]
        X,

        /// <summary>
        /// Memory addressed by the ES:eDI or by RDI (only MOVS, CMPS, INS, STOS, and SCAS). In 64-bit mode,
        /// only 64-bit (RDI) and 32-bit (EDI) address sizes are supported. In non-64-bit modes, only 32-bit (EDI)
        /// and 16-bit (DI) address sizes are supported. The implicit ES segment register cannot be overriden by
        /// a segment prefix.
        /// </summary>
        [XmlEnum(Name = "Y")]
        Y,

        /// <summary>
        /// Stack operand, used by instructions which either push an operand to the stack or pop an operand
        /// from the stack. Pop-like instructions are, for example, POP, RET, IRET, LEAVE. Push-like are, for
        /// example, PUSH, CALL, INT. No Operand type is provided along with this method because it depends
        /// on source/destination operand(s).
        /// </summary>
        [XmlEnum(Name = "SC")]
        SC,

        /// <summary>
        /// The two bits at bit index three of the opcode byte selects one of original four segment registers
        /// (for example, PUSH ES).
        /// </summary>
        [XmlEnum(Name = "S2")]
        S2,

        /// <summary>
        /// The three least-significant bits of the opcode byte selects segment register SS, FS, or GS (for example, LSS).
        /// </summary>
        [XmlEnum(Name = "S30")]
        S30,

        /// <summary>
        /// The three bits at bit index three of the opcode byte selects segment register FS or GS (for example, PUSH FS).
        /// </summary>
        [XmlEnum(Name = "S33")]
        S33,

        /// <summary>
        /// rFLAGS register.
        /// </summary>
        [XmlEnum(Name = "F")]
        F,

        /// <summary>
        /// Immediate data. The operand value is encoded in subsequent bytes of the instruction.
        /// </summary>
        [XmlEnum(Name = "I")]
        I,

        /// <summary>
        /// (Implies original E). A ModR/M byte follows the opcode and specifies the x87 FPU stack register.
        /// </summary>
        [XmlEnum(Name = "EST")]
        EST,
    }
}
