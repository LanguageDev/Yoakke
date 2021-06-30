// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.X86
{
    /// <summary>
    /// The default x86 <see cref="Register"/>s.
    /// </summary>
    public static class Registers
    {
        /* 8 bits */

        public static readonly Register Al = new("al", 1);
        public static readonly Register Ah = new("ah", 1);
        public static readonly Register Cl = new("cl", 1);
        public static readonly Register Ch = new("ch", 1);
        public static readonly Register Dl = new("dl", 1);
        public static readonly Register Dh = new("dh", 1);
        public static readonly Register Bl = new("bl", 1);
        public static readonly Register Bh = new("bh", 1);
        public static readonly Register Spl = new("spl", 1);
        public static readonly Register Bpl = new("bpl", 1);
        public static readonly Register Sil = new("sil", 1);
        public static readonly Register Dil = new("dil", 1);

        /* 16 bits */

        public static readonly Register Ax = new("ax", 2, Al, Ah);
        public static readonly Register Cx = new("cx", 2, Cl, Ch);
        public static readonly Register Dx = new("dx", 2, Dl, Dh);
        public static readonly Register Bx = new("bx", 2, Bl, Bh);
        public static readonly Register Sp = new("sp", 2, Spl);
        public static readonly Register Bp = new("bp", 2, Bpl);
        public static readonly Register Si = new("si", 2, Sil);
        public static readonly Register Di = new("di", 2, Dil);

        /* 32 bits */

        public static readonly Register Eax = new("eax", 4, Ax);
        public static readonly Register Ecx = new("ecx", 4, Cx);
        public static readonly Register Edx = new("edx", 4, Dx);
        public static readonly Register Ebx = new("ebx", 4, Bx);
        public static readonly Register Esp = new("esp", 4, Sp);
        public static readonly Register Ebp = new("ebp", 4, Bp);
        public static readonly Register Esi = new("esi", 4, Si);
        public static readonly Register Edi = new("edi", 4, Di);

        /* 64 bits */

        public static readonly Register Rax = new("rax", 8, Eax);
        public static readonly Register Rcx = new("rcx", 8, Ecx);
        public static readonly Register Rdx = new("rdx", 8, Edx);
        public static readonly Register Rbx = new("rbx", 8, Ebx);
        public static readonly Register Rsp = new("rsp", 8, Esp);
        public static readonly Register Rbp = new("rbp", 8, Ebp);
        public static readonly Register Rsi = new("rsi", 8, Esi);
        public static readonly Register Rdi = new("rdi", 8, Edi);
        public static readonly Register R8 = new("r8", 8);
        public static readonly Register R9 = new("r9", 8);
        public static readonly Register R10 = new("r10", 8);
        public static readonly Register R11 = new("r11", 8);
        public static readonly Register R12 = new("r12", 8);
        public static readonly Register R13 = new("r13", 8);
        public static readonly Register R14 = new("r14", 8);
        public static readonly Register R15 = new("r15", 8);
    }
}
