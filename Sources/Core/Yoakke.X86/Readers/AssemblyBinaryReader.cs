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

        /// <summary>
        /// Buffer for reading in new bytes from the <see cref="Underlying"/> reader into the <see cref="peekBuffer"/>.
        /// </summary>
        private readonly byte[] readBuffer;

        /// <summary>
        /// The buffer that saves the read in bytes without immediately consuming them.
        /// </summary>
        private readonly RingBuffer<byte> peekBuffer;

        /// <summary>
        /// The buffer holding the currently read in prefixes.
        /// </summary>
        private readonly byte[] prefixBuffer;

        /// <summary>
        /// The offset inside <see cref="peekBuffer"/>.
        /// </summary>
        private int offset;

        /// <summary>
        /// True, if the <see cref="Underlying"/> has reached the end.
        /// </summary>
        private bool ended;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyBinaryReader"/> class.
        /// </summary>
        /// <param name="reader">The underlying <see cref="BinaryReader"/> to read from.</param>
        public AssemblyBinaryReader(BinaryReader reader)
        {
            this.Underlying = reader;
            this.readBuffer = new byte[16];
            this.peekBuffer = new();
            this.prefixBuffer = new byte[4];
        }

        /// <summary>
        /// Reads the next <see cref="IInstruction"/> from the <see cref="Underlying"/> reader.
        /// </summary>
        /// <param name="length">The number of read bytes will be written here, so the caller can know the exact
        /// byte-length of the parser <see cref="IInstruction"/>.</param>
        /// <returns>The parsed <see cref="IInstruction"/>, or null, if the end of <see cref="Underlying"/> is reached.</returns>
        public IInstruction? ReadNext(out int length)
        {
            // Parse prefixes
            var prefixCount = this.ParsePrefixes();
            // Generated code uses this
            bool HasPrefix(byte b)
            {
                for (var i = 0; i < prefixCount; ++i)
                {
                    if (this.prefixBuffer[i] == b) return true;
                }
                return false;
            }

            byte modrm_byte;

            #region Generated

            if (this.TryParseByte(out var byte0))
            {
                switch (byte0)
                {
                case 0x00:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Add(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x01:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Add(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Add(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x02:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x03:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x04:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Add(Registers.Al, imm0_2);
                }
                case 0x05:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Add(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Add(Registers.Eax, imm0_2);
                }
                case 0x08:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Or(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x09:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Or(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Or(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x0a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x0b:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x0c:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Or(Registers.Al, imm0_2);
                }
                case 0x0d:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Or(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Or(Registers.Eax, imm0_2);
                }
                case 0x0f:
                {
                    if (this.TryParseByte(out var byte1))
                    {
                        switch (byte1)
                        {
                        case 0x01:
                        {
                            if (this.TryParseByte(out var byte2))
                            {
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
                            }
                            break;
                        }
                        case 0x0b:
                        {
                            length = this.Commit();
                            return new Instructions.Ud2();
                        }
                        case 0x0d:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetch(rm6);
                                }
                                case 0x01:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetchw(rm6);
                                }
                                case 0x02:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetchwt1(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x0e:
                        {
                            length = this.Commit();
                            return new Instructions.Femms();
                        }
                        case 0x18:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetchnta(rm6);
                                }
                                case 0x01:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetcht0(rm6);
                                }
                                case 0x02:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetcht1(rm6);
                                }
                                case 0x03:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Prefetcht2(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x2c:
                        {
                            if (HasPrefix(0xf2))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                    length = this.Commit();
                                    return new Instructions.Cvttsd2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
                            }
                            if (HasPrefix(0xf3))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Cvttss2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
                            }
                            break;
                        }
                        case 0x2d:
                        {
                            if (HasPrefix(0xf2))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                    length = this.Commit();
                                    return new Instructions.Cvtsd2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
                            }
                            if (HasPrefix(0xf3))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Cvtss2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
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
                            if (this.TryParseByte(out var byte2))
                            {
                                switch (byte2)
                                {
                                case 0xf0:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        if (this.TryParseByte(out modrm_byte))
                                        {
                                            var rm8 = this.ParseRM(modrm_byte, DataWidth.Word);
                                            length = this.Commit();
                                            return new Instructions.Movbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm8);
                                        }
                                    }
                                    if (HasPrefix(0xf2))
                                    {
                                        if (this.TryParseByte(out modrm_byte))
                                        {
                                            var rm8 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                            length = this.Commit();
                                            return new Instructions.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm8);
                                        }
                                    }
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Movbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm7);
                                    }
                                    break;
                                }
                                case 0xf1:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        if (HasPrefix(0xf2))
                                        {
                                            if (this.TryParseByte(out modrm_byte))
                                            {
                                                var rm9 = this.ParseRM(modrm_byte, DataWidth.Word);
                                                length = this.Commit();
                                                return new Instructions.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm9);
                                            }
                                        }
                                        if (this.TryParseByte(out modrm_byte))
                                        {
                                            var rm8 = this.ParseRM(modrm_byte, DataWidth.Word);
                                            length = this.Commit();
                                            return new Instructions.Movbe(rm8, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                        }
                                    }
                                    if (HasPrefix(0xf2))
                                    {
                                        if (this.TryParseByte(out modrm_byte))
                                        {
                                            var rm8 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                            length = this.Commit();
                                            return new Instructions.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm8);
                                        }
                                    }
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Movbe(rm7, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                                    }
                                    break;
                                }
                                case 0xf6:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        if (this.TryParseByte(out modrm_byte))
                                        {
                                            var rm8 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                            length = this.Commit();
                                            return new Instructions.Adcx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm8);
                                        }
                                    }
                                    if (HasPrefix(0xf3))
                                    {
                                        if (this.TryParseByte(out modrm_byte))
                                        {
                                            var rm8 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                            length = this.Commit();
                                            return new Instructions.Adox(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm8);
                                        }
                                    }
                                    break;
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x40:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovo(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovo(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x41:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovno(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovno(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x42:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnae(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnae(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x43:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovae(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovae(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x44:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmove(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovz(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmove(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovz(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x45:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovne(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnz(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovne(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnz(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x46:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovna(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovna(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x47:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmova(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmova(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x48:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovs(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovs(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x49:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovns(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovns(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x4a:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovpe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovpe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x4b:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovpo(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovpo(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x4c:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovl(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnge(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovl(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnge(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x4d:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovge(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnl(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovge(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnl(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x4e:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovle(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovng(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovle(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovng(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x4f:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmovnle(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnle(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0x77:
                        {
                            length = this.Commit();
                            return new Instructions.Emms();
                        }
                        case 0x80:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jo(rel0_4);
                        }
                        case 0x81:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jno(rel0_4);
                        }
                        case 0x82:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jb(rel0_4);
                        }
                        case 0x83:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jae(rel0_4);
                        }
                        case 0x84:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Je(rel0_4);
                        }
                        case 0x85:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jne(rel0_4);
                        }
                        case 0x86:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jbe(rel0_4);
                        }
                        case 0x87:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Ja(rel0_4);
                        }
                        case 0x88:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Js(rel0_4);
                        }
                        case 0x89:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jns(rel0_4);
                        }
                        case 0x8a:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jp(rel0_4);
                        }
                        case 0x8b:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jnp(rel0_4);
                        }
                        case 0x8c:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jl(rel0_4);
                        }
                        case 0x8d:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jge(rel0_4);
                        }
                        case 0x8e:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jle(rel0_4);
                        }
                        case 0x8f:
                        {
                            var rel0_4 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jg(rel0_4);
                        }
                        case 0x90:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Seto(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x91:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setno(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x92:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setb(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x93:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setae(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x94:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Sete(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x95:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setne(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x96:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setbe(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x97:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Seta(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x98:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Sets(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x99:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setns(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x9a:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setp(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x9b:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setnp(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x9c:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setl(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x9d:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setge(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x9e:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setle(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x9f:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setg(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
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
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Bt(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bt(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xa4:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    var imm0_6 = this.ParseImmediate(DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Shld(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), imm0_6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Shld(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), imm0_5);
                            }
                            break;
                        }
                        case 0xa5:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Shld(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), Registers.Cl);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Shld(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), Registers.Cl);
                            }
                            break;
                        }
                        case 0xab:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Bts(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bts(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xac:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    var imm0_6 = this.ParseImmediate(DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Shrd(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), imm0_6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Shrd(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), imm0_5);
                            }
                            break;
                        }
                        case 0xad:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Shrd(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), Registers.Cl);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Shrd(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), Registers.Cl);
                            }
                            break;
                        }
                        case 0xae:
                        {
                            if (this.TryParseByte(out var byte2))
                            {
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
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x02:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Ldmxcsr(rm6);
                                }
                                case 0x03:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Stmxcsr(rm6);
                                }
                                case 0x06:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Clwb(rm7);
                                    }
                                    break;
                                }
                                case 0x07:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Clflushopt(rm7);
                                    }
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Clflush(rm6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0xaf:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xb0:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Cmpxchg(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                            }
                            break;
                        }
                        case 0xb1:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Cmpxchg(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmpxchg(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xb3:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Btr(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Btr(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xb6:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xb7:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xb8:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (HasPrefix(0xf3))
                                {
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        length = this.Commit();
                                        return new Instructions.Popcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm7);
                                    }
                                }
                            }
                            if (HasPrefix(0xf3))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Popcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
                            }
                            break;
                        }
                        case 0xba:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x04:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        var imm0_7 = this.ParseImmediate(DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Bt(rm7, imm0_7);
                                    }
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    var imm0_6 = this.ParseImmediate(DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Bt(rm6, imm0_6);
                                }
                                case 0x05:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        var imm0_7 = this.ParseImmediate(DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Bts(rm7, imm0_7);
                                    }
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    var imm0_6 = this.ParseImmediate(DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Bts(rm6, imm0_6);
                                }
                                case 0x06:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        var imm0_7 = this.ParseImmediate(DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Btr(rm7, imm0_7);
                                    }
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    var imm0_6 = this.ParseImmediate(DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Btr(rm6, imm0_6);
                                }
                                case 0x07:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        var imm0_7 = this.ParseImmediate(DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Btc(rm7, imm0_7);
                                    }
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    var imm0_6 = this.ParseImmediate(DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Btc(rm6, imm0_6);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0xbb:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Btc(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Btc(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xbc:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (HasPrefix(0xf3))
                                {
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        length = this.Commit();
                                        return new Instructions.Tzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm7);
                                    }
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Bsf(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (HasPrefix(0xf3))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Tzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bsf(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xbd:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (HasPrefix(0xf3))
                                {
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        var rm7 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        length = this.Commit();
                                        return new Instructions.Lzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm7);
                                    }
                                }
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Bsr(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (HasPrefix(0xf3))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Lzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bsr(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xbe:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm6);
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xbf:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm5);
                            }
                            break;
                        }
                        case 0xc0:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Xadd(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                            }
                            break;
                        }
                        case 0xc1:
                        {
                            if (HasPrefix(0x66))
                            {
                                if (this.TryParseByte(out modrm_byte))
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Word);
                                    length = this.Commit();
                                    return new Instructions.Xadd(rm6, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                                }
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Xadd(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xc3:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Movnti(rm5, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xc7:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x01:
                                {
                                    var rm6 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                    length = this.Commit();
                                    return new Instructions.Cmpxchg8b(rm6);
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
                            }
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
                    }
                    break;
                }
                case 0x10:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Adc(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x11:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Adc(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Adc(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x12:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x13:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x14:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Adc(Registers.Al, imm0_2);
                }
                case 0x15:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Adc(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Adc(Registers.Eax, imm0_2);
                }
                case 0x18:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sbb(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x19:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Sbb(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sbb(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x1a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x1b:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x1c:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sbb(Registers.Al, imm0_2);
                }
                case 0x1d:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sbb(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sbb(Registers.Eax, imm0_2);
                }
                case 0x20:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.And(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x21:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.And(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.And(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x22:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x23:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x24:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.And(Registers.Al, imm0_2);
                }
                case 0x25:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.And(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.And(Registers.Eax, imm0_2);
                }
                case 0x27:
                {
                    length = this.Commit();
                    return new Instructions.Daa();
                }
                case 0x28:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sub(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x29:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Sub(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sub(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x2a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x2b:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x2c:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sub(Registers.Al, imm0_2);
                }
                case 0x2d:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Sub(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sub(Registers.Eax, imm0_2);
                }
                case 0x2f:
                {
                    length = this.Commit();
                    return new Instructions.Das();
                }
                case 0x30:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xor(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x31:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Xor(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xor(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x32:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x33:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x34:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Xor(Registers.Al, imm0_2);
                }
                case 0x35:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Xor(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Xor(Registers.Eax, imm0_2);
                }
                case 0x37:
                {
                    length = this.Commit();
                    return new Instructions.Aaa();
                }
                case 0x38:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Cmp(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x39:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Cmp(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Cmp(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x3a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x3b:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x3c:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Cmp(Registers.Al, imm0_2);
                }
                case 0x3d:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Cmp(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmp(Registers.Eax, imm0_2);
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
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Push(imm0_2);
                }
                case 0x69:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            var imm0_4 = this.ParseImmediate(DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4, imm0_4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        var imm0_3 = this.ParseImmediate(DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3, imm0_3);
                    }
                    break;
                }
                case 0x6a:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Push(imm0_2);
                }
                case 0x6b:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4, imm0_4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        var imm0_3 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3, imm0_3);
                    }
                    break;
                }
                case 0x70:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jo(rel0_2);
                }
                case 0x71:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jno(rel0_2);
                }
                case 0x72:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jb(rel0_2);
                }
                case 0x73:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jae(rel0_2);
                }
                case 0x74:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Je(rel0_2);
                }
                case 0x75:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jne(rel0_2);
                }
                case 0x76:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jbe(rel0_2);
                }
                case 0x77:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Ja(rel0_2);
                }
                case 0x78:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Js(rel0_2);
                }
                case 0x79:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jns(rel0_2);
                }
                case 0x7a:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jp(rel0_2);
                }
                case 0x7b:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jnp(rel0_2);
                }
                case 0x7c:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jl(rel0_2);
                }
                case 0x7d:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jge(rel0_2);
                }
                case 0x7e:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jle(rel0_2);
                }
                case 0x7f:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jg(rel0_2);
                }
                case 0x80:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Add(rm4, imm0_4);
                        }
                        case 0x01:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Or(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Adc(rm4, imm0_4);
                        }
                        case 0x03:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sbb(rm4, imm0_4);
                        }
                        case 0x04:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.And(rm4, imm0_4);
                        }
                        case 0x05:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sub(rm4, imm0_4);
                        }
                        case 0x06:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Xor(rm4, imm0_4);
                        }
                        case 0x07:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Cmp(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0x81:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Add(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Add(rm4, imm0_4);
                        }
                        case 0x01:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Or(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Or(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Adc(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Adc(rm4, imm0_4);
                        }
                        case 0x03:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Sbb(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Sbb(rm4, imm0_4);
                        }
                        case 0x04:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.And(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.And(rm4, imm0_4);
                        }
                        case 0x05:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Sub(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Sub(rm4, imm0_4);
                        }
                        case 0x06:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Xor(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Xor(rm4, imm0_4);
                        }
                        case 0x07:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Cmp(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Cmp(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0x83:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Add(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Add(rm4, imm0_4);
                        }
                        case 0x01:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Or(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Or(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Adc(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Adc(rm4, imm0_4);
                        }
                        case 0x03:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Sbb(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sbb(rm4, imm0_4);
                        }
                        case 0x04:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.And(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.And(rm4, imm0_4);
                        }
                        case 0x05:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Sub(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sub(rm4, imm0_4);
                        }
                        case 0x06:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Xor(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Xor(rm4, imm0_4);
                        }
                        case 0x07:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Cmp(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Cmp(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0x84:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Test(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x85:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Test(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Test(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x86:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xchg(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x87:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Xchg(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xchg(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x88:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Mov(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x89:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Mov(rm4, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word));
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Mov(rm3, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
                    }
                    break;
                }
                case 0x8a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), rm3);
                    }
                    break;
                }
                case 0x8b:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Word);
                            length = this.Commit();
                            return new Instructions.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x8d:
                {
                    if (HasPrefix(0x66))
                    {
                        if (this.TryParseByte(out modrm_byte))
                        {
                            var rm4 = this.ParseRM(modrm_byte, null);
                            length = this.Commit();
                            return new Instructions.Lea(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), rm4);
                        }
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        var rm3 = this.ParseRM(modrm_byte, null);
                        length = this.Commit();
                        return new Instructions.Lea(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), rm3);
                    }
                    break;
                }
                case 0x8f:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Pop(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Pop(rm4);
                        }
                        }
                        this.UnparseByte();
                    }
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
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Test(Registers.Al, imm0_2);
                }
                case 0xa9:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Test(Registers.Ax, imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Test(Registers.Eax, imm0_2);
                }
                case 0xb0:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb1:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb2:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb3:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb4:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb5:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb6:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb7:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), imm0_2);
                }
                case 0xb8:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xb9:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xba:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xbb:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xbc:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xbd:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xbe:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xbf:
                {
                    if (HasPrefix(0x66))
                    {
                        var imm0_3 = this.ParseImmediate(DataWidth.Word);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Word), imm0_3);
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Dword), imm0_2);
                }
                case 0xc0:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rol(rm4, imm0_4);
                        }
                        case 0x01:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Ror(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcl(rm4, imm0_4);
                        }
                        case 0x03:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcr(rm4, imm0_4);
                        }
                        case 0x04:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sal(rm4, imm0_4);
                        }
                        case 0x05:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Shr(rm4, imm0_4);
                        }
                        case 0x07:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sar(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xc1:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Rol(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rol(rm4, imm0_4);
                        }
                        case 0x01:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Ror(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Ror(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Rcl(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcl(rm4, imm0_4);
                        }
                        case 0x03:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Rcr(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcr(rm4, imm0_4);
                        }
                        case 0x04:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Sal(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sal(rm4, imm0_4);
                        }
                        case 0x05:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Shr(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Shr(rm4, imm0_4);
                        }
                        case 0x07:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Sar(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sar(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xc2:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Ret(imm0_2);
                }
                case 0xc3:
                {
                    length = this.Commit();
                    return new Instructions.Ret();
                }
                case 0xc6:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Mov(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xc7:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Mov(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Mov(rm4, imm0_4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xc8:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Word);
                    var imm1_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Enter(imm0_2, imm1_2);
                }
                case 0xc9:
                {
                    length = this.Commit();
                    return new Instructions.Leave();
                }
                case 0xcc:
                {
                    length = this.Commit();
                    return new Instructions.Int(new Constant(3));
                }
                case 0xcd:
                {
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Int(imm0_2);
                }
                case 0xce:
                {
                    length = this.Commit();
                    return new Instructions.Into();
                }
                case 0xd0:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rol(rm4, new Constant(1));
                        }
                        case 0x01:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Ror(rm4, new Constant(1));
                        }
                        case 0x02:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcl(rm4, new Constant(1));
                        }
                        case 0x03:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcr(rm4, new Constant(1));
                        }
                        case 0x04:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sal(rm4, new Constant(1));
                        }
                        case 0x05:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Shr(rm4, new Constant(1));
                        }
                        case 0x07:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sar(rm4, new Constant(1));
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xd1:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Rol(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Rol(rm4, new Constant(1));
                        }
                        case 0x01:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Ror(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Ror(rm4, new Constant(1));
                        }
                        case 0x02:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Rcl(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Rcl(rm4, new Constant(1));
                        }
                        case 0x03:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Rcr(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Rcr(rm4, new Constant(1));
                        }
                        case 0x04:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Sal(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Sal(rm4, new Constant(1));
                        }
                        case 0x05:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Shr(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Shr(rm4, new Constant(1));
                        }
                        case 0x07:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Sar(rm5, new Constant(1));
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Sar(rm4, new Constant(1));
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xd2:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rol(rm4, Registers.Cl);
                        }
                        case 0x01:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Ror(rm4, Registers.Cl);
                        }
                        case 0x02:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcl(rm4, Registers.Cl);
                        }
                        case 0x03:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Rcr(rm4, Registers.Cl);
                        }
                        case 0x04:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sal(rm4, Registers.Cl);
                        }
                        case 0x05:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Shr(rm4, Registers.Cl);
                        }
                        case 0x07:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Sar(rm4, Registers.Cl);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xd3:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Rol(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Rol(rm4, Registers.Cl);
                        }
                        case 0x01:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Ror(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Ror(rm4, Registers.Cl);
                        }
                        case 0x02:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Rcl(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Rcl(rm4, Registers.Cl);
                        }
                        case 0x03:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Rcr(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Rcr(rm4, Registers.Cl);
                        }
                        case 0x04:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Sal(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Sal(rm4, Registers.Cl);
                        }
                        case 0x05:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Shr(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Shr(rm4, Registers.Cl);
                        }
                        case 0x07:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Sar(rm5, Registers.Cl);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Sar(rm4, Registers.Cl);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xd4:
                {
                    if (this.TryParseByte(out var byte1))
                    {
                        switch (byte1)
                        {
                        case 0x0a:
                        {
                            length = this.Commit();
                            return new Instructions.Aam();
                        }
                        }
                        this.UnparseByte();
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Aam(imm0_2);
                }
                case 0xd5:
                {
                    if (this.TryParseByte(out var byte1))
                    {
                        switch (byte1)
                        {
                        case 0x0a:
                        {
                            length = this.Commit();
                            return new Instructions.Aad();
                        }
                        }
                        this.UnparseByte();
                    }
                    var imm0_2 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Aad(imm0_2);
                }
                case 0xd7:
                {
                    length = this.Commit();
                    return new Instructions.Xlatb();
                }
                case 0xe3:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jecxz(rel0_2);
                }
                case 0xe8:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Call(rel0_2);
                }
                case 0xe9:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Jmp(rel0_2);
                }
                case 0xeb:
                {
                    var rel0_2 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jmp(rel0_2);
                }
                case 0xf5:
                {
                    length = this.Commit();
                    return new Instructions.Cmc();
                }
                case 0xf6:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            var imm0_4 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Test(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Not(rm4);
                        }
                        case 0x03:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Neg(rm4);
                        }
                        case 0x04:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Mul(rm4);
                        }
                        case 0x05:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Imul(rm4);
                        }
                        case 0x06:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Div(rm4);
                        }
                        case 0x07:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Idiv(rm4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xf7:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                var imm0_5 = this.ParseImmediate(DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Test(rm5, imm0_5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            var imm0_4 = this.ParseImmediate(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Test(rm4, imm0_4);
                        }
                        case 0x02:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Not(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Not(rm4);
                        }
                        case 0x03:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Neg(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Neg(rm4);
                        }
                        case 0x04:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Mul(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Mul(rm4);
                        }
                        case 0x05:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Imul(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Imul(rm4);
                        }
                        case 0x06:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Div(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Div(rm4);
                        }
                        case 0x07:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Idiv(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Idiv(rm4);
                        }
                        }
                        this.UnparseByte();
                    }
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
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Inc(rm4);
                        }
                        case 0x01:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Dec(rm4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xff:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Inc(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Inc(rm4);
                        }
                        case 0x01:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Dec(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Dec(rm4);
                        }
                        case 0x02:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Call(rm4);
                        }
                        case 0x04:
                        {
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jmp(rm4);
                        }
                        case 0x06:
                        {
                            if (HasPrefix(0x66))
                            {
                                var rm5 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Push(rm5);
                            }
                            var rm4 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Push(rm4);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                }
                this.UnparseByte();
            }

            #endregion Generated

            if (this.ended)
            {
                length = 0;
                return null;
            }

            // TODO: We couldn't match an instruction
            throw new NotImplementedException();
        }

        private bool TryParseByte(out byte result)
        {
            // NOTE: We only need to look ahead one, hopefully this is correct
            if (this.peekBuffer.Count <= this.offset)
            {
                if (this.ended)
                {
                    // We can't even read anymore
                    result = default;
                    return false;
                }
                // We can try to read in more bytes
                var readCount = this.Underlying.Read(this.readBuffer);
                this.ended = readCount < this.readBuffer.Length;
                // Query the read in bytes
                for (var i = 0; i < readCount; ++i) this.peekBuffer.AddBack(this.readBuffer[i]);
                // This still does not guarantee that we have a byte read in, we could have just hit a border
                if (this.peekBuffer.Count <= this.offset)
                {
                    result = default;
                    return false;
                }
            }
            // Here we are guaranteed to have the byte
            result = this.peekBuffer[this.offset];
            ++this.offset;
            return true;
        }

        private void UnparseByte() => --this.offset;

        private int ParsePrefixes()
        {
            var i = 0;
            for (; i < this.prefixBuffer.Length; ++i)
            {
                // If there's no byte to read, return
                if (!this.TryParseByte(out var prefix)) break;
                if (prefix != 0x66)
                {
                    // Wasn't a prefix
                    this.UnparseByte();
                    break;
                }
                // Was a prefix, write it in
                this.prefixBuffer[i] = prefix;
            }
            return i;
        }

        private int Commit()
        {
            var result = this.offset;
            for (var i = 0; i < this.offset; ++i) this.peekBuffer.RemoveFront();
            this.offset = 0;
            return result;
        }

        // TODO: Review these methods, immediates should be unsigned by default, while relative offsets should be signed

        /* Operand parsers */

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

        private IOperand ParseRM(byte modrm, DataWidth? width)
        {
            // TODO: Segment override prefixes?
            // TODO: We probably need to use the width a few places, rn. all register sizes are hardcoded

            var mode = modrm >> 6;
            var rm = modrm & 0b111;

            // We parse SIB byte in case rm is 100 in any mode but 11
            var sib_base = 0;
            ScaledIndex? scaledIndex = null;
            if (mode != 0b11 && rm == 0b100)
            {
                // TODO: Not the best way to ignore the problem
                this.TryParseByte(out var sib);
                var sib_scale = sib >> 6;
                var sib_index = (sib >> 3) & 0b111;
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

        private int ParseNumber(DataWidth width)
        {
            var bytes = new byte[(int)width];
            for (var i = 0; i < bytes.Length; ++i)
            {
                // TODO: Not the best way to just ignore it
                if (!this.TryParseByte(out var b)) break;
                bytes[i] = b;
            }

            return width switch
            {
                DataWidth.Byte => bytes[0],
                DataWidth.Word => BitConverter.ToInt16(bytes),
                DataWidth.Dword => BitConverter.ToInt32(bytes),
                _ => throw new NotImplementedException(),
            };
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
