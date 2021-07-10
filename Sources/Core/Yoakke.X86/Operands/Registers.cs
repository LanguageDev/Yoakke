// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.X86.Operands
{
    /// <summary>
    /// The default x86 <see cref="Register"/>s.
    /// </summary>
    public static class Registers
    {
        /* Segment */

        public static readonly Segment Es = new("es");
        public static readonly Segment Cs = new("cs");
        public static readonly Segment Ss = new("ss");
        public static readonly Segment Ds = new("ds");
        public static readonly Segment Fs = new("fs");
        public static readonly Segment Gs = new("gs");

        /* 8 bits */

        public static readonly Register Al = new("al", DataWidth.Byte);
        public static readonly Register Ah = new("ah", DataWidth.Byte);
        public static readonly Register Cl = new("cl", DataWidth.Byte);
        public static readonly Register Ch = new("ch", DataWidth.Byte);
        public static readonly Register Dl = new("dl", DataWidth.Byte);
        public static readonly Register Dh = new("dh", DataWidth.Byte);
        public static readonly Register Bl = new("bl", DataWidth.Byte);
        public static readonly Register Bh = new("bh", DataWidth.Byte);
        public static readonly Register Spl = new("spl", DataWidth.Byte);
        public static readonly Register Bpl = new("bpl", DataWidth.Byte);
        public static readonly Register Sil = new("sil", DataWidth.Byte);
        public static readonly Register Dil = new("dil", DataWidth.Byte);

        /* 16 bits */

        public static readonly Register Ax = new("ax", DataWidth.Word, Al, Ah);
        public static readonly Register Cx = new("cx", DataWidth.Word, Cl, Ch);
        public static readonly Register Dx = new("dx", DataWidth.Word, Dl, Dh);
        public static readonly Register Bx = new("bx", DataWidth.Word, Bl, Bh);
        public static readonly Register Sp = new("sp", DataWidth.Word, Spl);
        public static readonly Register Bp = new("bp", DataWidth.Word, Bpl);
        public static readonly Register Si = new("si", DataWidth.Word, Sil);
        public static readonly Register Di = new("di", DataWidth.Word, Dil);

        /* 32 bits */

        public static readonly Register Eax = new("eax", DataWidth.Dword, Ax);
        public static readonly Register Ecx = new("ecx", DataWidth.Dword, Cx);
        public static readonly Register Edx = new("edx", DataWidth.Dword, Dx);
        public static readonly Register Ebx = new("ebx", DataWidth.Dword, Bx);
        public static readonly Register Esp = new("esp", DataWidth.Dword, Sp);
        public static readonly Register Ebp = new("ebp", DataWidth.Dword, Bp);
        public static readonly Register Esi = new("esi", DataWidth.Dword, Si);
        public static readonly Register Edi = new("edi", DataWidth.Dword, Di);

        /* 64 bits */

        public static readonly Register Rax = new("rax", DataWidth.Qword, Eax);
        public static readonly Register Rcx = new("rcx", DataWidth.Qword, Ecx);
        public static readonly Register Rdx = new("rdx", DataWidth.Qword, Edx);
        public static readonly Register Rbx = new("rbx", DataWidth.Qword, Ebx);
        public static readonly Register Rsp = new("rsp", DataWidth.Qword, Esp);
        public static readonly Register Rbp = new("rbp", DataWidth.Qword, Ebp);
        public static readonly Register Rsi = new("rsi", DataWidth.Qword, Esi);
        public static readonly Register Rdi = new("rdi", DataWidth.Qword, Edi);
        public static readonly Register R8 = new("r8", DataWidth.Qword);
        public static readonly Register R9 = new("r9", DataWidth.Qword);
        public static readonly Register R10 = new("r10", DataWidth.Qword);
        public static readonly Register R11 = new("r11", DataWidth.Qword);
        public static readonly Register R12 = new("r12", DataWidth.Qword);
        public static readonly Register R13 = new("r13", DataWidth.Qword);
        public static readonly Register R14 = new("r14", DataWidth.Qword);
        public static readonly Register R15 = new("r15", DataWidth.Qword);
    }
}
