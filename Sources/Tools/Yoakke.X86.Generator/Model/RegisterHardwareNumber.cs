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
    /// The hardware number of direct register operands.
    /// </summary>
    public enum RegisterHardwareNumber
    {
        /* Regular hardware registers */

        /// <summary>
        /// Accumulator register.
        /// AL, AX, EAX, RAX.
        /// </summary>
        [XmlEnum(Name = "0")]
        Accumulator,

        /// <summary>
        /// Counter register.
        /// CL, CX, ECX, RCX.
        /// </summary>
        [XmlEnum(Name = "1")]
        Counter,

        /// <summary>
        /// Data register.
        /// DL, DX, EDX, RDX.
        /// </summary>
        [XmlEnum(Name = "2")]
        Data,

        /// <summary>
        /// Base register.
        /// BL, BX, EBX, RBX.
        /// </summary>
        [XmlEnum(Name = "3")]
        Base,

        /// <summary>
        /// Stack Pointer register.
        /// AH, SP, ESP, RSP.
        /// </summary>
        [XmlEnum(Name = "4")]
        StackPointer,

        /// <summary>
        /// Stack Base Pointer register.
        /// SP, ESP, RSP.
        /// </summary>
        [XmlEnum(Name = "5")]
        StackBase,

        /// <summary>
        /// Source Index register.
        /// SI, ESI, RSI.
        /// </summary>
        [XmlEnum(Name = "6")]
        SourceIndex,

        /// <summary>
        /// Destination Index Register.
        /// </summary>
        [XmlEnum(Name = "7")]
        DestinationIndex,

        /// <summary>
        /// Register 8.
        /// </summary>
        [XmlEnum(Name = "8")]
        R8,

        /// <summary>
        /// Register 9.
        /// </summary>
        [XmlEnum(Name = "9")]
        R9,

        /// <summary>
        /// Register 10.
        /// </summary>
        [XmlEnum(Name = "10")]
        R10,

        /// <summary>
        /// Register 11.
        /// </summary>
        [XmlEnum(Name = "11")]
        R11,

        /// <summary>
        /// Register 12.
        /// </summary>
        [XmlEnum(Name = "12")]
        R12,

        /// <summary>
        /// Register 13.
        /// </summary>
        [XmlEnum(Name = "13")]
        R13,

        /// <summary>
        /// Register 14.
        /// </summary>
        [XmlEnum(Name = "14")]
        R14,

        /// <summary>
        /// Register 15.
        /// </summary>
        [XmlEnum(Name = "15")]
        R15,

        /* No idea what these are */

        /// <summary>
        /// IA32_SYSENTER_CS.
        /// </summary>
        [XmlEnum(Name = "174")]
        Ia32SysenterCs,

        /// <summary>
        /// IA32_SYSENTER_ESP.
        /// </summary>
        [XmlEnum(Name = "175")]
        Ia32SysenterEsp,

        /// <summary>
        /// IA32_SYSENTER_EIP.
        /// </summary>
        [XmlEnum(Name = "176")]
        Ia32SysenterEip,

        /* Some weird ring things */

        /// <summary>
        /// IA32_KERNEL_GSBASE.
        /// </summary>
        [XmlEnum(Name = "C0000102")]
        Ia32KernelGsBase,

        /// <summary>
        /// IA32_TSC_AUX.
        /// </summary>
        [XmlEnum(Name = "C0000103")]
        Ia32TscAux,

        /// <summary>
        /// IA32_FMASK.
        /// </summary>
        [XmlEnum(Name = "C0000084")]
        Ia32Fmask,

        /// <summary>
        /// IA32_LSTAR.
        /// </summary>
        [XmlEnum(Name = "C0000082")]
        Ia32Lstar,

        /// <summary>
        /// IA32_STAR.
        /// </summary>
        [XmlEnum(Name = "C0000081")]
        Ia32Star,

        /// <summary>
        /// IA32_BIOS_SIGN_ID.
        /// </summary>
        [XmlEnum(Name = "8B")]
        Ia32BiosSignId,
    }
}
