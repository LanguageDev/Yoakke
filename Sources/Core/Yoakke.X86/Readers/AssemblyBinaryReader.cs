// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Utilities;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Readers
{
    /// <summary>
    /// A reader that reads assembly instructions from binary code.
    /// </summary>
    public class AssemblyBinaryReader
    {
        /// <summary>
        /// The underlying <see cref="BinaryReader"/>.
        /// </summary>
        public BinaryReader Underlying { get; }

        private readonly RingBuffer<byte> peekBuffer;
        private int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBinaryReader"/> class.
        /// </summary>
        /// <param name="reader">The underlying <see cref="BinaryReader"/> to read from.</param>
        public AssemblyBinaryReader(BinaryReader reader)
        {
            this.Underlying = reader;
            this.peekBuffer = new();
        }

        /// <summary>
        /// Reads the next <see cref="IInstruction"/> from the <see cref="Underlying"/> reader.
        /// </summary>
        /// <param name="length">The number of read bytes will be written here, so the caller can know the exact
        /// byte-length of the parser <see cref="IInstruction"/>.</param>
        /// <returns>The parsed <see cref="IInstruction"/>.</returns>
        public IInstruction ReadNext(out int length)
        {
            // Take prefixes, 4 at most
            var prefixes = this.ParsePrefixes();
            // Generated code uses this
            bool HasPrefix(byte b) => prefixes!.Contains(b);

            // TODO: EOF?
            var byte0 = this.ParseByte();

            #region Generated

            switch (byte0)
            {
            case 0x00:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Add(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x01:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Add(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Add(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x02:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Add(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x03:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Add(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Add(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x04:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Add(Registers.Al, imm0_1);
            }
            case 0x05:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Add(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Add(Registers.Eax, imm0_1);
            }
            case 0x08:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Or(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x09:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Or(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Or(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x0a:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Or(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x0b:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Or(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Or(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x0c:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Or(Registers.Al, imm0_1);
            }
            case 0x0d:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Or(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Or(Registers.Eax, imm0_1);
            }
            case 0x0f:
            {
                var byte1 = this.ParseByte();
                switch (byte1)
                {
                case 0x01:
                {
                    var byte2 = this.ParseByte();
                    switch (byte2)
                    {
                    case 0xc8:
                    {
                        length = this.Commit();
                        return new Instructions.Monitor();
                    }
                    case 0xc9:
                    {
                        length = this.Commit();
                        return new Instructions.Mwait();
                    }
                    case 0xd0:
                    {
                        length = this.Commit();
                        return new Instructions.Xgetbv();
                    }
                    case 0xf9:
                    {
                        length = this.Commit();
                        return new Instructions.Rdtscp();
                    }
                    case 0xfa:
                    {
                        length = this.Commit();
                        return new Instructions.Monitorx();
                    }
                    case 0xfb:
                    {
                        length = this.Commit();
                        return new Instructions.Mwaitx();
                    }
                    case 0xfc:
                    {
                        length = this.Commit();
                        return new Instructions.Clzero();
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x0b:
                {
                    length = this.Commit();
                    return new Instructions.Ud2();
                }
                case 0x0d:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetch(rm3);
                    }
                    case 0x01:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetchw(rm3);
                    }
                    case 0x02:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetchwt1(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x0e:
                {
                    length = this.Commit();
                    return new Instructions.Femms();
                }
                case 0x18:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetchnta(rm3);
                    }
                    case 0x01:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetcht0(rm3);
                    }
                    case 0x02:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetcht1(rm3);
                    }
                    case 0x03:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Prefetcht2(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x2c:
                {
                    if (HasPrefix(0xf2))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Qword);
                        length = this.Commit();
                        return new Instructions.Cvttsd2si(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    if (HasPrefix(0xf3))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Cvttss2si(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x2d:
                {
                    if (HasPrefix(0xf2))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Qword);
                        length = this.Commit();
                        return new Instructions.Cvtsd2si(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    if (HasPrefix(0xf3))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Cvtss2si(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x31:
                {
                    length = this.Commit();
                    return new Instructions.Rdtsc();
                }
                case 0x38:
                {
                    var byte2 = this.ParseByte();
                    switch (byte2)
                    {
                    case 0xf0:
                    {
                        if (HasPrefix(0x66))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Movbe(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                        if (HasPrefix(0xf2))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Crc32(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Dword), rm4);
                        }
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Movbe(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    case 0xf1:
                    {
                        if (HasPrefix(0x66))
                        {
                            if (HasPrefix(0xf2))
                            {
                                var modrm5 = this.ParseByte();
                                var rm5 = this.ParseRM(modrm5, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Crc32(FromRegisterIndex((modrm5 >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Movbe(rm4, FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Word));
                        }
                        if (HasPrefix(0xf2))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Crc32(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Dword), rm4);
                        }
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Movbe(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword));
                    }
                    case 0xf6:
                    {
                        if (HasPrefix(0x66))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Adcx(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Dword), rm4);
                        }
                        if (HasPrefix(0xf3))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Adox(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Dword), rm4);
                        }
                        break;
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x40:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovo(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovo(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x41:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovno(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovno(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x42:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovb(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovb(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x43:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovae(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovae(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x44:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmove(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmove(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x45:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovne(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovne(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x46:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovbe(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovbe(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x47:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmova(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmova(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x48:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovs(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovs(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x49:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovns(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovns(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x4a:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovp(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovp(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x4b:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovnp(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovnp(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x4c:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovl(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovl(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x4d:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovge(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovge(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x4e:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovle(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovle(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x4f:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmovg(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmovg(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0x77:
                {
                    length = this.Commit();
                    return new Instructions.Emms();
                }
                case 0x90:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Seto(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x91:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setno(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x92:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setb(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x93:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setae(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x94:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sete(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x95:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setne(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x96:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setbe(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x97:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Seta(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x98:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sets(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x99:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setns(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x9a:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setp(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x9b:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setnp(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x9c:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setl(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x9d:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setge(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x9e:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setle(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0x9f:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x00:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Setg(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0xa2:
                {
                    length = this.Commit();
                    return new Instructions.Cpuid();
                }
                case 0xa3:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Bt(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word));
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Bt(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xa4:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Shld(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), imm0_3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Shld(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xa5:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Shld(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), Registers.Cl);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Shld(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), Registers.Cl);
                }
                case 0xab:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Bts(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word));
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Bts(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xac:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Shrd(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), imm0_3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Shrd(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xad:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Shrd(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), Registers.Cl);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Shrd(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), Registers.Cl);
                }
                case 0xae:
                {
                    var byte2 = this.ParseByte();
                    switch (byte2)
                    {
                    case 0xe8:
                    {
                        length = this.Commit();
                        return new Instructions.Lfence();
                    }
                    case 0xf0:
                    {
                        length = this.Commit();
                        return new Instructions.Mfence();
                    }
                    case 0xf8:
                    {
                        length = this.Commit();
                        return new Instructions.Sfence();
                    }
                    }
                    this.UnparseByte();
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x02:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Ldmxcsr(rm3);
                    }
                    case 0x03:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Stmxcsr(rm3);
                    }
                    case 0x06:
                    {
                        if (HasPrefix(0x66))
                        {
                            var rm4 = this.ParseRM(modrm2, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Clwb(rm4);
                        }
                        break;
                    }
                    case 0x07:
                    {
                        if (HasPrefix(0x66))
                        {
                            var rm4 = this.ParseRM(modrm2, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Clflushopt(rm4);
                        }
                        var rm3 = this.ParseRM(modrm2, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Clflush(rm3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0xaf:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Imul(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Imul(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xb0:
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Cmpxchg(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Byte));
                }
                case 0xb1:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmpxchg(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word));
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmpxchg(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xb3:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Btr(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word));
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Btr(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xb6:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Movzx(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Movzx(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xb7:
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Movzx(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xb8:
                {
                    if (HasPrefix(0x66))
                    {
                        if (HasPrefix(0xf3))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Popcnt(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (HasPrefix(0xf3))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Popcnt(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0xba:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x04:
                    {
                        if (HasPrefix(0x66))
                        {
                            var rm4 = this.ParseRM(modrm2, DataWidth.Word);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Bt(rm4, imm0_4);
                        }
                        var rm3 = this.ParseRM(modrm2, DataWidth.Dword);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Bt(rm3, imm0_3);
                    }
                    case 0x05:
                    {
                        if (HasPrefix(0x66))
                        {
                            var rm4 = this.ParseRM(modrm2, DataWidth.Word);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Bts(rm4, imm0_4);
                        }
                        var rm3 = this.ParseRM(modrm2, DataWidth.Dword);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Bts(rm3, imm0_3);
                    }
                    case 0x06:
                    {
                        if (HasPrefix(0x66))
                        {
                            var rm4 = this.ParseRM(modrm2, DataWidth.Word);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Btr(rm4, imm0_4);
                        }
                        var rm3 = this.ParseRM(modrm2, DataWidth.Dword);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Btr(rm3, imm0_3);
                    }
                    case 0x07:
                    {
                        if (HasPrefix(0x66))
                        {
                            var rm4 = this.ParseRM(modrm2, DataWidth.Word);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Btc(rm4, imm0_4);
                        }
                        var rm3 = this.ParseRM(modrm2, DataWidth.Dword);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Btc(rm3, imm0_3);
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0xbb:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Btc(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word));
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Btc(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xbc:
                {
                    if (HasPrefix(0x66))
                    {
                        if (HasPrefix(0xf3))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Tzcnt(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Bsf(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    if (HasPrefix(0xf3))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Tzcnt(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Bsf(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xbd:
                {
                    if (HasPrefix(0x66))
                    {
                        if (HasPrefix(0xf3))
                        {
                            var modrm4 = this.ParseByte();
                            var rm4 = this.ParseRM(modrm4, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Lzcnt(FromRegisterIndex((modrm4 >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Bsr(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    if (HasPrefix(0xf3))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Lzcnt(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Bsr(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xbe:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Movsx(FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word), rm3);
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Movsx(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xbf:
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Movsx(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword), rm2);
                }
                case 0xc0:
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Xadd(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Byte));
                }
                case 0xc1:
                {
                    if (HasPrefix(0x66))
                    {
                        var modrm3 = this.ParseByte();
                        var rm3 = this.ParseRM(modrm3, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Xadd(rm3, FromRegisterIndex((modrm3 >> 3) & 0b111, DataWidth.Word));
                    }
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Xadd(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xc3:
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Movnti(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Dword));
                }
                case 0xc7:
                {
                    var modrm2 = this.ParseByte();
                    switch ((modrm2 >> 3) & 0b111)
                    {
                    case 0x01:
                    {
                        var rm3 = this.ParseRM(modrm2, DataWidth.Qword);
                        length = this.Commit();
                        return new Instructions.Cmpxchg8b(rm3);
                    }
                    case 0x06:
                    {
                        if (HasPrefix(0x66))
                        {
                        }
                        break;
                    }
                    case 0x07:
                    {
                        if (HasPrefix(0x66))
                        {
                        }
                        break;
                    }
                    }
                    this.UnparseByte();
                    break;
                }
                case 0xc8:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xc9:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xca:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xcb:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xcc:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xcd:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xce:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                case 0xcf:
                {
                    length = this.Commit();
                    return new Instructions.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
                }
                }
                this.UnparseByte();
                break;
            }
            case 0x10:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Adc(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x11:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Adc(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Adc(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x12:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Adc(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x13:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Adc(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Adc(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x14:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Adc(Registers.Al, imm0_1);
            }
            case 0x15:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Adc(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Adc(Registers.Eax, imm0_1);
            }
            case 0x18:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Sbb(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x19:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Sbb(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Sbb(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x1a:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Sbb(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x1b:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Sbb(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Sbb(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x1c:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Sbb(Registers.Al, imm0_1);
            }
            case 0x1d:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Sbb(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Sbb(Registers.Eax, imm0_1);
            }
            case 0x20:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.And(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x21:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.And(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.And(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x22:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.And(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x23:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.And(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.And(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x24:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.And(Registers.Al, imm0_1);
            }
            case 0x25:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.And(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.And(Registers.Eax, imm0_1);
            }
            case 0x27:
            {
                length = this.Commit();
                return new Instructions.Daa();
            }
            case 0x28:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Sub(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x29:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Sub(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Sub(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x2a:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Sub(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x2b:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Sub(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Sub(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x2c:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Sub(Registers.Al, imm0_1);
            }
            case 0x2d:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Sub(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Sub(Registers.Eax, imm0_1);
            }
            case 0x2f:
            {
                length = this.Commit();
                return new Instructions.Das();
            }
            case 0x30:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Xor(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x31:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Xor(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Xor(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x32:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Xor(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x33:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Xor(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Xor(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x34:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Xor(Registers.Al, imm0_1);
            }
            case 0x35:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Xor(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Xor(Registers.Eax, imm0_1);
            }
            case 0x37:
            {
                length = this.Commit();
                return new Instructions.Aaa();
            }
            case 0x38:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Cmp(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x39:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Cmp(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Cmp(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x3a:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Cmp(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x3b:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Cmp(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Cmp(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x3c:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Cmp(Registers.Al, imm0_1);
            }
            case 0x3d:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Cmp(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Cmp(Registers.Eax, imm0_1);
            }
            case 0x3f:
            {
                length = this.Commit();
                return new Instructions.Aas();
            }
            case 0x40:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x41:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x42:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x43:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x44:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x45:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x46:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x47:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x48:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x49:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x4a:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x4b:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x4c:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x4d:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x4e:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x4f:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x50:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x51:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x52:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x53:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x54:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x55:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x56:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x57:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x58:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x59:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x5a:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x5b:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x5c:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x5d:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x5e:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x5f:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x68:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Push(imm0_1);
            }
            case 0x69:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Imul(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2, imm0_2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Imul(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1, imm0_1);
            }
            case 0x6a:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Push(imm0_1);
            }
            case 0x6b:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Imul(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2, imm0_2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Imul(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1, imm0_1);
            }
            case 0x80:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Add(rm2, imm0_2);
                }
                case 0x01:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Or(rm2, imm0_2);
                }
                case 0x02:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Adc(rm2, imm0_2);
                }
                case 0x03:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sbb(rm2, imm0_2);
                }
                case 0x04:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.And(rm2, imm0_2);
                }
                case 0x05:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sub(rm2, imm0_2);
                }
                case 0x06:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Xor(rm2, imm0_2);
                }
                case 0x07:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Cmp(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0x81:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Add(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Add(rm2, imm0_2);
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Or(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Or(rm2, imm0_2);
                }
                case 0x02:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Adc(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Adc(rm2, imm0_2);
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sbb(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sbb(rm2, imm0_2);
                }
                case 0x04:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.And(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.And(rm2, imm0_2);
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sub(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sub(rm2, imm0_2);
                }
                case 0x06:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Xor(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Xor(rm2, imm0_2);
                }
                case 0x07:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmp(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmp(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0x83:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Add(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Add(rm2, imm0_2);
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Or(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Or(rm2, imm0_2);
                }
                case 0x02:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Adc(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Adc(rm2, imm0_2);
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sbb(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sbb(rm2, imm0_2);
                }
                case 0x04:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.And(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.And(rm2, imm0_2);
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sub(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sub(rm2, imm0_2);
                }
                case 0x06:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xor(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Xor(rm2, imm0_2);
                }
                case 0x07:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Cmp(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Cmp(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0x84:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Test(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x85:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Test(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Test(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x86:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Xchg(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x87:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Xchg(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Xchg(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x88:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte));
            }
            case 0x89:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(rm2, FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word));
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(rm1, FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword));
            }
            case 0x8a:
            {
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Byte), rm1);
            }
            case 0x8b:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x8d:
            {
                if (HasPrefix(0x66))
                {
                    var modrm2 = this.ParseByte();
                    var rm2 = this.ParseRM(modrm2, null);
                    length = this.Commit();
                    return new Instructions.Lea(FromRegisterIndex((modrm2 >> 3) & 0b111, DataWidth.Word), rm2);
                }
                var modrm1 = this.ParseByte();
                var rm1 = this.ParseRM(modrm1, null);
                length = this.Commit();
                return new Instructions.Lea(FromRegisterIndex((modrm1 >> 3) & 0b111, DataWidth.Dword), rm1);
            }
            case 0x8f:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Pop(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Pop(rm2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0x90:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                if (HasPrefix(0xf3))
                {
                    length = this.Commit();
                    return new Instructions.Pause();
                }
                length = this.Commit();
                return new Instructions.Nop();
            }
            case 0x91:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x92:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x93:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x94:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x95:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x96:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x97:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                }
                length = this.Commit();
                return new Instructions.Xchg(Registers.Eax, FromRegisterIndex(byte0 & 0b111, DataWidth.Dword));
            }
            case 0x98:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Cbw();
                }
                length = this.Commit();
                return new Instructions.Cwde();
            }
            case 0x99:
            {
                if (HasPrefix(0x66))
                {
                    length = this.Commit();
                    return new Instructions.Cwd();
                }
                length = this.Commit();
                return new Instructions.Cdq();
            }
            case 0x9e:
            {
                length = this.Commit();
                return new Instructions.Sahf();
            }
            case 0x9f:
            {
                length = this.Commit();
                return new Instructions.Lahf();
            }
            case 0xa8:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Test(Registers.Al, imm0_1);
            }
            case 0xa9:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Test(Registers.Ax, imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Test(Registers.Eax, imm0_1);
            }
            case 0xb0:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb1:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb2:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb3:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb4:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb5:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb6:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb7:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_1);
            }
            case 0xb8:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xb9:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xba:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xbb:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xbc:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xbd:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xbe:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xbf:
            {
                if (HasPrefix(0x66))
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_2);
                }
                var imm0_1 = this.ParseImmediate(DataWidth.Dword);
                length = this.Commit();
                return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_1);
            }
            case 0xc0:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rol(rm2, imm0_2);
                }
                case 0x01:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Ror(rm2, imm0_2);
                }
                case 0x02:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcl(rm2, imm0_2);
                }
                case 0x03:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcr(rm2, imm0_2);
                }
                case 0x04:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sal(rm2, imm0_2);
                }
                case 0x05:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Shr(rm2, imm0_2);
                }
                case 0x07:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sar(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xc1:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Rol(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rol(rm2, imm0_2);
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Ror(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Ror(rm2, imm0_2);
                }
                case 0x02:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Rcl(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcl(rm2, imm0_2);
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Rcr(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcr(rm2, imm0_2);
                }
                case 0x04:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sal(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sal(rm2, imm0_2);
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Shr(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Shr(rm2, imm0_2);
                }
                case 0x07:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sar(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sar(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xc2:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Word);
                length = this.Commit();
                return new Instructions.Ret(imm0_1);
            }
            case 0xc3:
            {
                length = this.Commit();
                return new Instructions.Ret();
            }
            case 0xc6:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xc7:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(rm2, imm0_2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xcc:
            {
                length = this.Commit();
                return new Instructions.Int(new Constant(3));
            }
            case 0xcd:
            {
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Int(imm0_1);
            }
            case 0xce:
            {
                length = this.Commit();
                return new Instructions.Into();
            }
            case 0xd0:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rol(rm2, new Constant(1));
                }
                case 0x01:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Ror(rm2, new Constant(1));
                }
                case 0x02:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcl(rm2, new Constant(1));
                }
                case 0x03:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcr(rm2, new Constant(1));
                }
                case 0x04:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sal(rm2, new Constant(1));
                }
                case 0x05:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Shr(rm2, new Constant(1));
                }
                case 0x07:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sar(rm2, new Constant(1));
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xd1:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Rol(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Rol(rm2, new Constant(1));
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Ror(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Ror(rm2, new Constant(1));
                }
                case 0x02:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Rcl(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Rcl(rm2, new Constant(1));
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Rcr(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Rcr(rm2, new Constant(1));
                }
                case 0x04:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sal(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sal(rm2, new Constant(1));
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Shr(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Shr(rm2, new Constant(1));
                }
                case 0x07:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sar(rm3, new Constant(1));
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sar(rm2, new Constant(1));
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xd2:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rol(rm2, Registers.Cl);
                }
                case 0x01:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Ror(rm2, Registers.Cl);
                }
                case 0x02:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcl(rm2, Registers.Cl);
                }
                case 0x03:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Rcr(rm2, Registers.Cl);
                }
                case 0x04:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sal(rm2, Registers.Cl);
                }
                case 0x05:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Shr(rm2, Registers.Cl);
                }
                case 0x07:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sar(rm2, Registers.Cl);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xd3:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Rol(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Rol(rm2, Registers.Cl);
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Ror(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Ror(rm2, Registers.Cl);
                }
                case 0x02:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Rcl(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Rcl(rm2, Registers.Cl);
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Rcr(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Rcr(rm2, Registers.Cl);
                }
                case 0x04:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sal(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sal(rm2, Registers.Cl);
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Shr(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Shr(rm2, Registers.Cl);
                }
                case 0x07:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sar(rm3, Registers.Cl);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sar(rm2, Registers.Cl);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xd4:
            {
                var byte1 = this.ParseByte();
                switch (byte1)
                {
                case 0x0a:
                {
                    length = this.Commit();
                    return new Instructions.Aam();
                }
                }
                this.UnparseByte();
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Aam(imm0_1);
            }
            case 0xd5:
            {
                var byte1 = this.ParseByte();
                switch (byte1)
                {
                case 0x0a:
                {
                    length = this.Commit();
                    return new Instructions.Aad();
                }
                }
                this.UnparseByte();
                var imm0_1 = this.ParseImmediate(DataWidth.Byte);
                length = this.Commit();
                return new Instructions.Aad(imm0_1);
            }
            case 0xd7:
            {
                length = this.Commit();
                return new Instructions.Xlatb();
            }
            case 0xf5:
            {
                length = this.Commit();
                return new Instructions.Cmc();
            }
            case 0xf6:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Test(rm2, imm0_2);
                }
                case 0x02:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Not(rm2);
                }
                case 0x03:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Neg(rm2);
                }
                case 0x04:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mul(rm2);
                }
                case 0x05:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Imul(rm2);
                }
                case 0x06:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Div(rm2);
                }
                case 0x07:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Idiv(rm2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xf7:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Test(rm3, imm0_3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Test(rm2, imm0_2);
                }
                case 0x02:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Not(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Not(rm2);
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Neg(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Neg(rm2);
                }
                case 0x04:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mul(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mul(rm2);
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Imul(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Imul(rm2);
                }
                case 0x06:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Div(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Div(rm2);
                }
                case 0x07:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Idiv(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Idiv(rm2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xf8:
            {
                length = this.Commit();
                return new Instructions.Clc();
            }
            case 0xf9:
            {
                length = this.Commit();
                return new Instructions.Stc();
            }
            case 0xfc:
            {
                length = this.Commit();
                return new Instructions.Cld();
            }
            case 0xfd:
            {
                length = this.Commit();
                return new Instructions.Std();
            }
            case 0xfe:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Inc(rm2);
                }
                case 0x01:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Dec(rm2);
                }
                }
                this.UnparseByte();
                break;
            }
            case 0xff:
            {
                var modrm1 = this.ParseByte();
                switch ((modrm1 >> 3) & 0b111)
                {
                case 0x00:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Inc(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Inc(rm2);
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Dec(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Dec(rm2);
                }
                case 0x02:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Call(rm2);
                }
                case 0x04:
                {
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Jmp(rm2);
                }
                case 0x06:
                {
                    if (HasPrefix(0x66))
                    {
                        var rm3 = this.ParseRM(modrm1, DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Push(rm3);
                    }
                    var rm2 = this.ParseRM(modrm1, DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Push(rm2);
                }
                }
                this.UnparseByte();
                break;
            }
            }
            this.UnparseByte();

            #endregion Generated

            // TODO: We couldn't match an instruction
            throw new NotImplementedException();
        }

        private byte[] ParsePrefixes()
        {
            // Take prefixes, 4 at most
            var prefixes = new byte[4];
            for (var i = 0; i < 4; ++i)
            {
                var prefix = this.ParseByte();
                // TODO: Support more prefixes
                if (prefix != 0x66)
                {
                    // Wasn't a prefix
                    this.UnparseByte();
                    break;
                }
                prefixes[i] = (byte)prefix;
            }
            return prefixes;
        }

        private byte ParseByte()
        {
            // NOTE: We only need to look ahead one, hopefully this is correct
            if (this.peekBuffer.Count <= this.offset)
            {
                // TODO: What if we are out of bytes?
                var read = this.Underlying.ReadByte();
                this.peekBuffer.AddBack((byte)read);
            }

            var result = this.peekBuffer[this.offset];
            ++this.offset;
            return result;
        }

        private void UnparseByte() => --this.offset;

        // TODO: Review these methods, immediates should be unsigned by default, while relative offsets should be signed

        private IOperand ParseImmediate(DataWidth width)
        {
            var number = this.ParseNumber(width);
            return new Constant(width, number);
        }

        private IOperand ParseCodeOffset(DataWidth width)
        {
            var number = this.ParseNumber(width);
            return new Constant(width, number);
        }

        private int ParseNumber(DataWidth width)
        {
            var bytes = new byte[(int)width];
            for (var i = 0; i < bytes.Length; ++i) bytes[i] = (byte)this.ParseByte();

            return width switch
            {
                DataWidth.Byte => bytes[0],
                DataWidth.Word => BitConverter.ToInt16(bytes),
                DataWidth.Dword => BitConverter.ToInt32(bytes),
                _ => throw new NotImplementedException(),
            };
        }

        private IOperand ParseRM(byte modrm, DataWidth? width)
        {
            // TODO: Segment override prefixes?
            // TODO: We probably need to use the width a few places, rn. all register sizes are hardcoded

            var mode = modrm >> 6;
            var rm = modrm & 0b111;

            // We parse SIB byte in case rm is 100 in any mode but 11
            var sib_scale = 0;
            var sib_index = 0;
            var sib_base = 0;
            ScaledIndex? scaledIndex = null;
            if (mode != 0b11 && rm == 0b100)
            {
                var sib = (byte)this.ParseByte();
                sib_scale = sib >> 6;
                sib_index = (sib >> 3) & 0b111;
                sib_base = sib & 0b111;
                scaledIndex = new ScaledIndex(FromRegisterIndex(sib_index, DataWidth.Dword), 1 << sib_scale);
            }

            IOperand op = mode switch
            {
                // Register indirect addressing mode or SIB without displacement
                0b00 => rm switch
                {
                    // SIB only
                    0b100 => sib_base == 0b101
                        // Displacement only
                        ? new Address
                        {
                            ScaledIndex = scaledIndex!.Value,
                            Displacement = this.ParseNumber(DataWidth.Dword),
                        }
                        // Base
                        : new Address
                        {
                            Base = FromRegisterIndex(sib_base, DataWidth.Dword),
                            ScaledIndex = scaledIndex!.Value,
                        },

                    // 32-bit displacement only
                    0b101 => new Address
                    {
                        Displacement = this.ParseNumber(DataWidth.Dword),
                    },

                    // Register-indirect
                    _ => new Address
                    {
                        Base = FromRegisterIndex(rm, DataWidth.Dword),
                    },
                },

                // 8 or 32 -bit displacement
                0b01 or 0b10 => rm == 0b100
                    // Has a SIB
                    ? new Address
                    {
                        Base = FromRegisterIndex(sib_base, DataWidth.Dword),
                        ScaledIndex = scaledIndex!.Value,
                        Displacement = this.ParseNumber(mode == 0b01 ? DataWidth.Byte : DataWidth.Dword),
                    }
                    // No SIB
                    : new Address
                    {
                        Base = FromRegisterIndex(rm, DataWidth.Dword),
                        Displacement = this.ParseNumber(mode == 0b01 ? DataWidth.Byte : DataWidth.Dword),
                    },

                // Register addressing mode
                0b11 => FromRegisterIndex(rm, width ?? throw new NotImplementedException()),

                _ => throw new InvalidOperationException(),
            };

            // If we have an address but a width was specified, that means it's an indirect read
            return op is Address addr && width is not null
                ? new Indirect(width.Value, addr)
                : op;
        }

        private int Commit()
        {
            var result = this.offset;
            for (var i = 0; i < this.offset; ++i) this.peekBuffer.RemoveFront();
            this.offset = 0;
            return result;
        }

        private static Register FromRegisterIndex(int index, DataWidth width) => width switch
        {
            DataWidth.Byte => Registers8Bits[index],
            DataWidth.Word => Registers16Bits[index],
            DataWidth.Dword => Registers32Bits[index],
            _ => throw new NotImplementedException(),
        };

        private static readonly Register[] Registers8Bits = new[]
        {
            Registers.Al, Registers.Cl, Registers.Dl, Registers.Bl,
            Registers.Ah, Registers.Ch, Registers.Dh, Registers.Bh,
        };

        private static readonly Register[] Registers16Bits = new[]
        {
            Registers.Ax, Registers.Cx, Registers.Dx, Registers.Bx,
            Registers.Sp, Registers.Bp, Registers.Si, Registers.Di,
        };

        private static readonly Register[] Registers32Bits = new[]
        {
            Registers.Eax, Registers.Ecx, Registers.Edx, Registers.Ebx,
            Registers.Esp, Registers.Ebp, Registers.Esi, Registers.Edi,
        };
    }
}
