// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Platform.X86.Operands;

/// <summary>
/// The default x86 <see cref="Register"/>s.
/// </summary>
public static class Registers
{
  /* Segment */

  /// <summary>
  /// Extra Segment.
  /// </summary>
  public static readonly Segment Es = new("es");

  /// <summary>
  /// Code Segment.
  /// </summary>
  public static readonly Segment Cs = new("cs");

  /// <summary>
  /// Stack Segment.
  /// </summary>
  public static readonly Segment Ss = new("ss");

  /// <summary>
  /// Data Segment.
  /// </summary>
  public static readonly Segment Ds = new("ds");

  /// <summary>
  /// General purpose Segment.
  /// </summary>
  public static readonly Segment Fs = new("fs");

  /// <summary>
  /// General purpose Segment.
  /// </summary>
  public static readonly Segment Gs = new("gs");

  /* 8 bits */

  /// <summary>
  /// Lower 8 bits of the 16 bit Accumulator register.
  /// </summary>
  public static readonly Register Al = new("al", DataWidth.Byte);

  /// <summary>
  /// Higher 8 bits of the 16 bit Accumulator register.
  /// </summary>
  public static readonly Register Ah = new("ah", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Counter register.
  /// </summary>
  public static readonly Register Cl = new("cl", DataWidth.Byte);

  /// <summary>
  /// Higher 8 bits of the 16 bit Counter register.
  /// </summary>
  public static readonly Register Ch = new("ch", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Data register.
  /// </summary>
  public static readonly Register Dl = new("dl", DataWidth.Byte);

  /// <summary>
  /// Higher 8 bits of the 16 bit Data register.
  /// </summary>
  public static readonly Register Dh = new("dh", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Base register.
  /// </summary>
  public static readonly Register Bl = new("bl", DataWidth.Byte);

  /// <summary>
  /// Higher 8 bits of the 16 bit Base register.
  /// </summary>
  public static readonly Register Bh = new("bh", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Stack Pointer register.
  /// </summary>
  public static readonly Register Spl = new("spl", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Stack Base Pointer register.
  /// </summary>
  public static readonly Register Bpl = new("bpl", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Source Index register.
  /// </summary>
  public static readonly Register Sil = new("sil", DataWidth.Byte);

  /// <summary>
  /// Lower 8 bits of the 16 bit Destination Index register.
  /// </summary>
  public static readonly Register Dil = new("dil", DataWidth.Byte);

  /* 16 bits */

  /// <summary>
  /// 16 bit Accumulator register.
  /// </summary>
  public static readonly Register Ax = new("ax", DataWidth.Word, Al, Ah);

  /// <summary>
  /// 16 bit Counter register.
  /// </summary>
  public static readonly Register Cx = new("cx", DataWidth.Word, Cl, Ch);

  /// <summary>
  /// 16 bit Data register.
  /// </summary>
  public static readonly Register Dx = new("dx", DataWidth.Word, Dl, Dh);

  /// <summary>
  /// 16 bit Base register.
  /// </summary>
  public static readonly Register Bx = new("bx", DataWidth.Word, Bl, Bh);

  /// <summary>
  /// 16 bit Stack Pointer register.
  /// </summary>
  public static readonly Register Sp = new("sp", DataWidth.Word, Spl);

  /// <summary>
  /// 16 bit Stack Base Pointer register.
  /// </summary>
  public static readonly Register Bp = new("bp", DataWidth.Word, Bpl);

  /// <summary>
  /// 16 bit Source Index register.
  /// </summary>
  public static readonly Register Si = new("si", DataWidth.Word, Sil);

  /// <summary>
  /// 16 bit Destination Index register.
  /// </summary>
  public static readonly Register Di = new("di", DataWidth.Word, Dil);

  /* 32 bits */

  /// <summary>
  /// 32 bit Accumulator register.
  /// </summary>
  public static readonly Register Eax = new("eax", DataWidth.Dword, Ax);

  /// <summary>
  /// 32 bit Counter register.
  /// </summary>
  public static readonly Register Ecx = new("ecx", DataWidth.Dword, Cx);

  /// <summary>
  /// 32 bit Data register.
  /// </summary>
  public static readonly Register Edx = new("edx", DataWidth.Dword, Dx);

  /// <summary>
  /// 32 bit Base register.
  /// </summary>
  public static readonly Register Ebx = new("ebx", DataWidth.Dword, Bx);

  /// <summary>
  /// 32 bit Stack Pointer register.
  /// </summary>
  public static readonly Register Esp = new("esp", DataWidth.Dword, Sp);

  /// <summary>
  /// 32 bit Stack Base Pointer register.
  /// </summary>
  public static readonly Register Ebp = new("ebp", DataWidth.Dword, Bp);

  /// <summary>
  /// 32 bit Source Index register.
  /// </summary>
  public static readonly Register Esi = new("esi", DataWidth.Dword, Si);

  /// <summary>
  /// 32 bit Destination Index register.
  /// </summary>
  public static readonly Register Edi = new("edi", DataWidth.Dword, Di);

  /* 64 bits */

  /// <summary>
  /// 64 bit Accumulator register.
  /// </summary>
  public static readonly Register Rax = new("rax", DataWidth.Qword, Eax);

  /// <summary>
  /// 64 bit Counter register.
  /// </summary>
  public static readonly Register Rcx = new("rcx", DataWidth.Qword, Ecx);

  /// <summary>
  /// 64 bit Data register.
  /// </summary>
  public static readonly Register Rdx = new("rdx", DataWidth.Qword, Edx);

  /// <summary>
  /// 64 bit Base register.
  /// </summary>
  public static readonly Register Rbx = new("rbx", DataWidth.Qword, Ebx);

  /// <summary>
  /// 64 bit Stack Pointer register.
  /// </summary>
  public static readonly Register Rsp = new("rsp", DataWidth.Qword, Esp);

  /// <summary>
  /// 64 bit Stack Base Pointer register.
  /// </summary>
  public static readonly Register Rbp = new("rbp", DataWidth.Qword, Ebp);

  /// <summary>
  /// 64 bit Source Index register.
  /// </summary>
  public static readonly Register Rsi = new("rsi", DataWidth.Qword, Esi);

  /// <summary>
  /// 64 bit Destination Index register.
  /// </summary>
  public static readonly Register Rdi = new("rdi", DataWidth.Qword, Edi);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R8 = new("r8", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R9 = new("r9", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R10 = new("r10", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R11 = new("r11", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R12 = new("r12", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R13 = new("r13", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R14 = new("r14", DataWidth.Qword);

  /// <summary>
  /// 64 bit general purpose register.
  /// </summary>
  public static readonly Register R15 = new("r15", DataWidth.Qword);
}
