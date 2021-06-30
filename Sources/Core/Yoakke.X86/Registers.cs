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

        public static readonly Register Al = new(1);
        public static readonly Register Ah = new(1);
        public static readonly Register Cl = new(1);
        public static readonly Register Ch = new(1);
        public static readonly Register Dl = new(1);
        public static readonly Register Dh = new(1);
        public static readonly Register Bl = new(1);
        public static readonly Register Bh = new(1);
        public static readonly Register Spl = new(1);
        public static readonly Register Bpl = new(1);
        public static readonly Register Sil = new(1);
        public static readonly Register Dil = new(1);

        /* 16 bits */

        public static readonly Register Ax = new(2, Al, Ah);
        public static readonly Register Cx = new(2, Cl, Ch);
        public static readonly Register Dx = new(2, Dl, Dh);
        public static readonly Register Bx = new(2, Bl, Bh);
        public static readonly Register Sp = new(2, Spl);
        public static readonly Register Bp = new(2, Bpl);
        public static readonly Register Si = new(2, Sil);
        public static readonly Register Di = new(2, Dil);

        /* 32 bits */

        public static readonly Register Eax = new(4, Ax);
        public static readonly Register Ecx = new(4, Cx);
        public static readonly Register Edx = new(4, Dx);
        public static readonly Register Ebx = new(4, Bx);
        public static readonly Register Esp = new(4, Sp);
        public static readonly Register Ebp = new(4, Bp);
        public static readonly Register Esi = new(4, Si);
        public static readonly Register Edi = new(4, Di);

        /* 64 bits */

        public static readonly Register Rax = new(8, Eax);
        public static readonly Register Rcx = new(8, Ecx);
        public static readonly Register Rdx = new(8, Edx);
        public static readonly Register Rbx = new(8, Ebx);
        public static readonly Register Rsp = new(8, Esp);
        public static readonly Register Rbp = new(8, Ebp);
        public static readonly Register Rsi = new(8, Esi);
        public static readonly Register Rdi = new(8, Edi);
        public static readonly Register R8 = new(8);
        public static readonly Register R9 = new(8);
        public static readonly Register R10 = new(8);
        public static readonly Register R11 = new(8);
        public static readonly Register R12 = new(8);
        public static readonly Register R13 = new(8);
        public static readonly Register R14 = new(8);
        public static readonly Register R15 = new(8);
    }
}
