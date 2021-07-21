// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;
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
        /// Tries to read the next <see cref="IInstruction"/> from the <see cref="Underlying"/> reader.
        /// </summary>
        /// <param name="instruction">The read instruction gets written here, if succeeded.</param>
        /// <param name="length">The number of read bytes will be written here, so the caller can know the exact
        /// byte-length of the parser <see cref="IInstruction"/>.</param>
        /// <returns>True, if there was something to read.</returns>
        public bool TryReadNext([MaybeNullWhen(false)] out IInstruction instruction, out int length)
        {
            instruction = this.ReadNext(out length);
            return instruction is not null;
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
            IOperand op0, op1, op2;

            #region Generated

            if (this.TryParseByte(out var byte0))
            {
                switch (byte0)
                {
                case 0x00:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Add(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x01:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Add(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x02:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x03:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x04:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Add(Registers.Al, op1);
                }
                case 0x05:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Add(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x08:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Or(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x09:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Or(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x0a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x0b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x0c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Or(Registers.Al, op1);
                }
                case 0x0d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Or(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
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
                                length = this.Commit();
                                switch (byte2)
                                {
                                case 0xc8:
                                {
                                    return new Instructions.Monitor();
                                }
                                case 0xc9:
                                {
                                    return new Instructions.Mwait();
                                }
                                case 0xd0:
                                {
                                    return new Instructions.Xgetbv();
                                }
                                case 0xf9:
                                {
                                    return new Instructions.Rdtscp();
                                }
                                case 0xfa:
                                {
                                    return new Instructions.Monitorx();
                                }
                                case 0xfb:
                                {
                                    return new Instructions.Mwaitx();
                                }
                                case 0xfc:
                                {
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
                                op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    return new Instructions.Prefetch(op0);
                                }
                                case 0x01:
                                {
                                    return new Instructions.Prefetchw(op0);
                                }
                                case 0x02:
                                {
                                    return new Instructions.Prefetchwt1(op0);
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
                                op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    return new Instructions.Prefetchnta(op0);
                                }
                                case 0x01:
                                {
                                    return new Instructions.Prefetcht0(op0);
                                }
                                case 0x02:
                                {
                                    return new Instructions.Prefetcht1(op0);
                                }
                                case 0x03:
                                {
                                    return new Instructions.Prefetcht2(op0);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x2c:
                        {
                            if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                length = this.Commit();
                                return new Instructions.Cvttsd2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cvttss2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x2d:
                        {
                            if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                length = this.Commit();
                                return new Instructions.Cvtsd2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cvtss2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
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
                                    if (HasPrefix(0x66) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Word);
                                        length = this.Commit();
                                        return new Instructions.Movbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), op1);
                                    }
                                    if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Movbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    break;
                                }
                                case 0xf1:
                                {
                                    if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Movbe(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                                    }
                                    break;
                                }
                                case 0xf6:
                                {
                                    if (HasPrefix(0x66) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Adcx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instructions.Adox(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
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
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovo(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x41:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovno(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x42:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovb(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovc(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnae(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x43:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovae(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnb(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnc(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x44:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmove(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovz(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x45:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovne(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnz(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x46:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovna(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x47:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmova(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x48:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovs(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x49:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovns(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4a:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovp(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovpe(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4b:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnp(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovpo(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4c:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovl(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnge(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4d:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovge(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnl(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4e:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovle(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovng(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4f:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovg(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmovnle(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
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
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jo(op0);
                        }
                        case 0x81:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jno(op0);
                        }
                        case 0x82:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jb(op0);
                        }
                        case 0x83:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jae(op0);
                        }
                        case 0x84:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Je(op0);
                        }
                        case 0x85:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jne(op0);
                        }
                        case 0x86:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jbe(op0);
                        }
                        case 0x87:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Ja(op0);
                        }
                        case 0x88:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Js(op0);
                        }
                        case 0x89:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jns(op0);
                        }
                        case 0x8a:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jp(op0);
                        }
                        case 0x8b:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jnp(op0);
                        }
                        case 0x8c:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jl(op0);
                        }
                        case 0x8d:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jge(op0);
                        }
                        case 0x8e:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jle(op0);
                        }
                        case 0x8f:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jg(op0);
                        }
                        case 0x90:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x00:
                                {
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Seto(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setno(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setb(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setae(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Sete(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setne(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setbe(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Seta(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Sets(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setns(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setp(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setnp(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setl(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setge(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setle(op0);
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Setg(op0);
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
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bt(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xa4:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                op2 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Shld(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op2);
                            }
                            break;
                        }
                        case 0xa5:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Shld(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), Registers.Cl);
                            }
                            break;
                        }
                        case 0xab:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bts(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xac:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                op2 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Shrd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op2);
                            }
                            break;
                        }
                        case 0xad:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Shrd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), Registers.Cl);
                            }
                            break;
                        }
                        case 0xae:
                        {
                            if (this.TryParseByte(out var byte2))
                            {
                                length = this.Commit();
                                switch (byte2)
                                {
                                case 0xe8:
                                {
                                    return new Instructions.Lfence();
                                }
                                case 0xf0:
                                {
                                    return new Instructions.Mfence();
                                }
                                case 0xf8:
                                {
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Ldmxcsr(op0);
                                }
                                case 0x03:
                                {
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instructions.Stmxcsr(op0);
                                }
                                case 0x06:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Clwb(op0);
                                    }
                                    break;
                                }
                                case 0x07:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instructions.Clflushopt(op0);
                                    }
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instructions.Clflush(op0);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0xaf:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xb0:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Cmpxchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                            }
                            break;
                        }
                        case 0xb1:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Cmpxchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xb3:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Btr(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xb6:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xb7:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xb8:
                        {
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Popcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xba:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                op1 = this.ParseImmediate(DataWidth.Byte);
                                length = this.Commit();
                                switch ((modrm_byte >> 3) & 0b111)
                                {
                                case 0x04:
                                {
                                    return new Instructions.Bt(op0, op1);
                                }
                                case 0x05:
                                {
                                    return new Instructions.Bts(op0, op1);
                                }
                                case 0x06:
                                {
                                    return new Instructions.Btr(op0, op1);
                                }
                                case 0x07:
                                {
                                    return new Instructions.Btc(op0, op1);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0xbb:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Btc(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xbc:
                        {
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Tzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bsf(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xbd:
                        {
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Lzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Bsr(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xbe:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xbf:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instructions.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xc0:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instructions.Xadd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                            }
                            break;
                        }
                        case 0xc1:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Xadd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xc3:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instructions.Movnti(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
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
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                    length = this.Commit();
                                    return new Instructions.Cmpxchg8b(op0);
                                }
                                case 0x06:
                                case 0x07:
                                {
                                    break;
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0xc8:
                        case 0xc9:
                        case 0xca:
                        case 0xcb:
                        case 0xcc:
                        case 0xcd:
                        case 0xce:
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Adc(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x11:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Adc(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x12:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x13:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x14:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Adc(Registers.Al, op1);
                }
                case 0x15:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Adc(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x18:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sbb(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x19:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sbb(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x1a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x1b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x1c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sbb(Registers.Al, op1);
                }
                case 0x1d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sbb(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x20:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.And(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x21:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.And(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x22:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x23:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x24:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.And(Registers.Al, op1);
                }
                case 0x25:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.And(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sub(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x29:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sub(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x2a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x2b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x2c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Sub(Registers.Al, op1);
                }
                case 0x2d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Sub(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xor(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x31:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xor(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x32:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x33:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x34:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Xor(Registers.Al, op1);
                }
                case 0x35:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Xor(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Cmp(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x39:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Cmp(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x3a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x3b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x3c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Cmp(Registers.Al, op1);
                }
                case 0x3d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Cmp(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x3f:
                {
                    length = this.Commit();
                    return new Instructions.Aas();
                }
                case 0x40:
                case 0x41:
                case 0x42:
                case 0x43:
                case 0x44:
                case 0x45:
                case 0x46:
                case 0x47:
                {
                    length = this.Commit();
                    return new Instructions.Inc(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                }
                case 0x48:
                case 0x49:
                case 0x4a:
                case 0x4b:
                case 0x4c:
                case 0x4d:
                case 0x4e:
                case 0x4f:
                {
                    length = this.Commit();
                    return new Instructions.Dec(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                }
                case 0x50:
                case 0x51:
                case 0x52:
                case 0x53:
                case 0x54:
                case 0x55:
                case 0x56:
                case 0x57:
                {
                    length = this.Commit();
                    return new Instructions.Push(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                }
                case 0x58:
                case 0x59:
                case 0x5a:
                case 0x5b:
                case 0x5c:
                case 0x5d:
                case 0x5e:
                case 0x5f:
                {
                    length = this.Commit();
                    return new Instructions.Pop(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                }
                case 0x68:
                {
                    op0 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Push(op0);
                }
                case 0x69:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op2 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1, op2);
                    }
                    break;
                }
                case 0x6a:
                {
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Push(op0);
                }
                case 0x6b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op2 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1, op2);
                    }
                    break;
                }
                case 0x70:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jo(op0);
                }
                case 0x71:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jno(op0);
                }
                case 0x72:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jb(op0);
                }
                case 0x73:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jae(op0);
                }
                case 0x74:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Je(op0);
                }
                case 0x75:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jne(op0);
                }
                case 0x76:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jbe(op0);
                }
                case 0x77:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Ja(op0);
                }
                case 0x78:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Js(op0);
                }
                case 0x79:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jns(op0);
                }
                case 0x7a:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jp(op0);
                }
                case 0x7b:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jnp(op0);
                }
                case 0x7c:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jl(op0);
                }
                case 0x7d:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jge(op0);
                }
                case 0x7e:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jle(op0);
                }
                case 0x7f:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jg(op0);
                }
                case 0x80:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        op1 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Add(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instructions.Or(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instructions.Adc(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instructions.Sbb(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instructions.And(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instructions.Sub(op0, op1);
                        }
                        case 0x06:
                        {
                            return new Instructions.Xor(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instructions.Cmp(op0, op1);
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
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Add(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instructions.Or(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instructions.Adc(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instructions.Sbb(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instructions.And(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instructions.Sub(op0, op1);
                        }
                        case 0x06:
                        {
                            return new Instructions.Xor(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instructions.Cmp(op0, op1);
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
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op1 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Add(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instructions.Or(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instructions.Adc(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instructions.Sbb(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instructions.And(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instructions.Sub(op0, op1);
                        }
                        case 0x06:
                        {
                            return new Instructions.Xor(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instructions.Cmp(op0, op1);
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Test(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x85:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Test(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x86:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Xchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x87:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Xchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x88:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Mov(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x89:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Mov(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x8a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x8b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instructions.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x8d:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, null);
                        length = this.Commit();
                        return new Instructions.Lea(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
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
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Pop(op0);
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
                case 0x92:
                case 0x93:
                case 0x94:
                case 0x95:
                case 0x96:
                case 0x97:
                {
                    length = this.Commit();
                    return new Instructions.Xchg(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
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
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Test(Registers.Al, op1);
                }
                case 0xa9:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Test(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0xb0:
                case 0xb1:
                case 0xb2:
                case 0xb3:
                case 0xb4:
                case 0xb5:
                case 0xb6:
                case 0xb7:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), op1);
                }
                case 0xb8:
                case 0xb9:
                case 0xba:
                case 0xbb:
                case 0xbc:
                case 0xbd:
                case 0xbe:
                case 0xbf:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Mov(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                }
                case 0xc0:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        op1 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Rol(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instructions.Ror(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instructions.Rcl(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instructions.Rcr(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instructions.Sal(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instructions.Shr(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instructions.Sar(op0, op1);
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
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op1 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Rol(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instructions.Ror(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instructions.Rcl(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instructions.Rcr(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instructions.Sal(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instructions.Shr(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instructions.Sar(op0, op1);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xc2:
                {
                    op0 = this.ParseImmediate(DataWidth.Word);
                    length = this.Commit();
                    return new Instructions.Ret(op0);
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
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            op1 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Mov(op0, op1);
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
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Mov(op0, op1);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xc8:
                {
                    op0 = this.ParseImmediate(DataWidth.Word);
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Enter(op0, op1);
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
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Int(op0);
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Rol(op0, new Constant(1));
                        }
                        case 0x01:
                        {
                            return new Instructions.Ror(op0, new Constant(1));
                        }
                        case 0x02:
                        {
                            return new Instructions.Rcl(op0, new Constant(1));
                        }
                        case 0x03:
                        {
                            return new Instructions.Rcr(op0, new Constant(1));
                        }
                        case 0x04:
                        {
                            return new Instructions.Sal(op0, new Constant(1));
                        }
                        case 0x05:
                        {
                            return new Instructions.Shr(op0, new Constant(1));
                        }
                        case 0x07:
                        {
                            return new Instructions.Sar(op0, new Constant(1));
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
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Rol(op0, new Constant(1));
                        }
                        case 0x01:
                        {
                            return new Instructions.Ror(op0, new Constant(1));
                        }
                        case 0x02:
                        {
                            return new Instructions.Rcl(op0, new Constant(1));
                        }
                        case 0x03:
                        {
                            return new Instructions.Rcr(op0, new Constant(1));
                        }
                        case 0x04:
                        {
                            return new Instructions.Sal(op0, new Constant(1));
                        }
                        case 0x05:
                        {
                            return new Instructions.Shr(op0, new Constant(1));
                        }
                        case 0x07:
                        {
                            return new Instructions.Sar(op0, new Constant(1));
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Rol(op0, Registers.Cl);
                        }
                        case 0x01:
                        {
                            return new Instructions.Ror(op0, Registers.Cl);
                        }
                        case 0x02:
                        {
                            return new Instructions.Rcl(op0, Registers.Cl);
                        }
                        case 0x03:
                        {
                            return new Instructions.Rcr(op0, Registers.Cl);
                        }
                        case 0x04:
                        {
                            return new Instructions.Sal(op0, Registers.Cl);
                        }
                        case 0x05:
                        {
                            return new Instructions.Shr(op0, Registers.Cl);
                        }
                        case 0x07:
                        {
                            return new Instructions.Sar(op0, Registers.Cl);
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
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Rol(op0, Registers.Cl);
                        }
                        case 0x01:
                        {
                            return new Instructions.Ror(op0, Registers.Cl);
                        }
                        case 0x02:
                        {
                            return new Instructions.Rcl(op0, Registers.Cl);
                        }
                        case 0x03:
                        {
                            return new Instructions.Rcr(op0, Registers.Cl);
                        }
                        case 0x04:
                        {
                            return new Instructions.Sal(op0, Registers.Cl);
                        }
                        case 0x05:
                        {
                            return new Instructions.Shr(op0, Registers.Cl);
                        }
                        case 0x07:
                        {
                            return new Instructions.Sar(op0, Registers.Cl);
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
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Aam(op0);
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
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Aad(op0);
                }
                case 0xd7:
                {
                    length = this.Commit();
                    return new Instructions.Xlatb();
                }
                case 0xe3:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jecxz(op0);
                }
                case 0xe8:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Call(op0);
                }
                case 0xe9:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Dword);
                    length = this.Commit();
                    return new Instructions.Jmp(op0);
                }
                case 0xeb:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instructions.Jmp(op0);
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
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            op1 = this.ParseImmediate(DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Test(op0, op1);
                        }
                        case 0x02:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Not(op0);
                        }
                        case 0x03:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Neg(op0);
                        }
                        case 0x04:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Mul(op0);
                        }
                        case 0x05:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Imul(op0);
                        }
                        case 0x06:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Div(op0);
                        }
                        case 0x07:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instructions.Idiv(op0);
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
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Test(op0, op1);
                        }
                        case 0x02:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Not(op0);
                        }
                        case 0x03:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Neg(op0);
                        }
                        case 0x04:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Mul(op0);
                        }
                        case 0x05:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Imul(op0);
                        }
                        case 0x06:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Div(op0);
                        }
                        case 0x07:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Idiv(op0);
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
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        switch ((modrm_byte >> 3) & 0b111)
                        {
                        case 0x00:
                        {
                            return new Instructions.Inc(op0);
                        }
                        case 0x01:
                        {
                            return new Instructions.Dec(op0);
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
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Inc(op0);
                        }
                        case 0x01:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Dec(op0);
                        }
                        case 0x02:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Call(op0);
                        }
                        case 0x04:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Jmp(op0);
                        }
                        case 0x06:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instructions.Push(op0);
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
