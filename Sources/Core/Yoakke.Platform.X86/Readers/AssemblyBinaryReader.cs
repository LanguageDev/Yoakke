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
using Yoakke.Platform.X86.Operands;

namespace Yoakke.Platform.X86.Readers
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
                        return new Instruction.Add(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x01:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Add(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x02:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x03:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Add(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x04:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Add(Registers.Al, op1);
                }
                case 0x05:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Add(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x08:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Or(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x09:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Or(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x0a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x0b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Or(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x0c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Or(Registers.Al, op1);
                }
                case 0x0d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Or(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
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
                                    return new Instruction.Monitor();
                                }
                                case 0xc9:
                                {
                                    return new Instruction.Mwait();
                                }
                                case 0xd0:
                                {
                                    return new Instruction.Xgetbv();
                                }
                                case 0xf9:
                                {
                                    return new Instruction.Rdtscp();
                                }
                                case 0xfa:
                                {
                                    return new Instruction.Monitorx();
                                }
                                case 0xfb:
                                {
                                    return new Instruction.Mwaitx();
                                }
                                case 0xfc:
                                {
                                    return new Instruction.Clzero();
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x0b:
                        {
                            length = this.Commit();
                            return new Instruction.Ud2();
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
                                    return new Instruction.Prefetch(op0);
                                }
                                case 0x01:
                                {
                                    return new Instruction.Prefetchw(op0);
                                }
                                case 0x02:
                                {
                                    return new Instruction.Prefetchwt1(op0);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0x0e:
                        {
                            length = this.Commit();
                            return new Instruction.Femms();
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
                                    return new Instruction.Prefetchnta(op0);
                                }
                                case 0x01:
                                {
                                    return new Instruction.Prefetcht0(op0);
                                }
                                case 0x02:
                                {
                                    return new Instruction.Prefetcht1(op0);
                                }
                                case 0x03:
                                {
                                    return new Instruction.Prefetcht2(op0);
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
                                return new Instruction.Cvttsd2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cvttss2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x2d:
                        {
                            if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Qword);
                                length = this.Commit();
                                return new Instruction.Cvtsd2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cvtss2si(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x31:
                        {
                            length = this.Commit();
                            return new Instruction.Rdtsc();
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
                                        return new Instruction.Movbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Word), op1);
                                    }
                                    if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instruction.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instruction.Movbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    break;
                                }
                                case 0xf1:
                                {
                                    if (HasPrefix(0xf2) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instruction.Crc32(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    if (this.TryParseByte(out modrm_byte))
                                    {
                                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instruction.Movbe(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                                    }
                                    break;
                                }
                                case 0xf6:
                                {
                                    if (HasPrefix(0x66) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instruction.Adcx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                                    }
                                    if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                                    {
                                        op1 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                        length = this.Commit();
                                        return new Instruction.Adox(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
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
                                return new Instruction.Cmovo(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x41:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovno(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x42:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovb(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovc(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnae(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x43:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovae(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnb(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnc(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x44:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmove(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovz(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x45:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovne(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnz(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x46:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovna(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x47:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmova(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnbe(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x48:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovs(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x49:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovns(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4a:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovp(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovpe(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4b:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnp(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovpo(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4c:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovl(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnge(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4d:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovge(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnl(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4e:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovle(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovng(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x4f:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovg(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmovnle(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0x77:
                        {
                            length = this.Commit();
                            return new Instruction.Emms();
                        }
                        case 0x80:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jo(op0);
                        }
                        case 0x81:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jno(op0);
                        }
                        case 0x82:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jb(op0);
                        }
                        case 0x83:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jae(op0);
                        }
                        case 0x84:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Je(op0);
                        }
                        case 0x85:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jne(op0);
                        }
                        case 0x86:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jbe(op0);
                        }
                        case 0x87:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Ja(op0);
                        }
                        case 0x88:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Js(op0);
                        }
                        case 0x89:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jns(op0);
                        }
                        case 0x8a:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jp(op0);
                        }
                        case 0x8b:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jnp(op0);
                        }
                        case 0x8c:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jl(op0);
                        }
                        case 0x8d:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jge(op0);
                        }
                        case 0x8e:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jle(op0);
                        }
                        case 0x8f:
                        {
                            op0 = this.ParseCodeOffset(DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jg(op0);
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
                                    return new Instruction.Seto(op0);
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
                                    return new Instruction.Setno(op0);
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
                                    return new Instruction.Setb(op0);
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
                                    return new Instruction.Setae(op0);
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
                                    return new Instruction.Sete(op0);
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
                                    return new Instruction.Setne(op0);
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
                                    return new Instruction.Setbe(op0);
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
                                    return new Instruction.Seta(op0);
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
                                    return new Instruction.Sets(op0);
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
                                    return new Instruction.Setns(op0);
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
                                    return new Instruction.Setp(op0);
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
                                    return new Instruction.Setnp(op0);
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
                                    return new Instruction.Setl(op0);
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
                                    return new Instruction.Setge(op0);
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
                                    return new Instruction.Setle(op0);
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
                                    return new Instruction.Setg(op0);
                                }
                                }
                                this.UnparseByte();
                            }
                            break;
                        }
                        case 0xa2:
                        {
                            length = this.Commit();
                            return new Instruction.Cpuid();
                        }
                        case 0xa3:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Bt(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
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
                                return new Instruction.Shld(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op2);
                            }
                            break;
                        }
                        case 0xa5:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Shld(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), Registers.Cl);
                            }
                            break;
                        }
                        case 0xab:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Bts(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
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
                                return new Instruction.Shrd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op2);
                            }
                            break;
                        }
                        case 0xad:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Shrd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), Registers.Cl);
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
                                    return new Instruction.Lfence();
                                }
                                case 0xf0:
                                {
                                    return new Instruction.Mfence();
                                }
                                case 0xf8:
                                {
                                    return new Instruction.Sfence();
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
                                    return new Instruction.Ldmxcsr(op0);
                                }
                                case 0x03:
                                {
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                    length = this.Commit();
                                    return new Instruction.Stmxcsr(op0);
                                }
                                case 0x06:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instruction.Clwb(op0);
                                    }
                                    break;
                                }
                                case 0x07:
                                {
                                    if (HasPrefix(0x66))
                                    {
                                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                        length = this.Commit();
                                        return new Instruction.Clflushopt(op0);
                                    }
                                    op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                    length = this.Commit();
                                    return new Instruction.Clflush(op0);
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
                                return new Instruction.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xb0:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instruction.Cmpxchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                            }
                            break;
                        }
                        case 0xb1:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Cmpxchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xb3:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Btr(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xb6:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instruction.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xb7:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instruction.Movzx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xb8:
                        {
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Popcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
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
                                    return new Instruction.Bt(op0, op1);
                                }
                                case 0x05:
                                {
                                    return new Instruction.Bts(op0, op1);
                                }
                                case 0x06:
                                {
                                    return new Instruction.Btr(op0, op1);
                                }
                                case 0x07:
                                {
                                    return new Instruction.Btc(op0, op1);
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
                                return new Instruction.Btc(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xbc:
                        {
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Tzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Bsf(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xbd:
                        {
                            if (HasPrefix(0xf3) && this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Lzcnt(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Bsr(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xbe:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instruction.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xbf:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op1 = this.ParseRM(modrm_byte, DataWidth.Word);
                                length = this.Commit();
                                return new Instruction.Movsx(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword), op1);
                            }
                            break;
                        }
                        case 0xc0:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                                length = this.Commit();
                                return new Instruction.Xadd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                            }
                            break;
                        }
                        case 0xc1:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Xadd(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                            }
                            break;
                        }
                        case 0xc3:
                        {
                            if (this.TryParseByte(out modrm_byte))
                            {
                                op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                                length = this.Commit();
                                return new Instruction.Movnti(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Dword));
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
                                    return new Instruction.Cmpxchg8b(op0);
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
                            return new Instruction.Bswap(FromRegisterIndex(byte1 & 0b111, DataWidth.Dword));
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
                        return new Instruction.Adc(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x11:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Adc(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x12:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x13:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Adc(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x14:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Adc(Registers.Al, op1);
                }
                case 0x15:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Adc(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x18:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Sbb(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x19:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Sbb(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x1a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x1b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Sbb(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x1c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Sbb(Registers.Al, op1);
                }
                case 0x1d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Sbb(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x20:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.And(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x21:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.And(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x22:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x23:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.And(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x24:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.And(Registers.Al, op1);
                }
                case 0x25:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.And(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x27:
                {
                    length = this.Commit();
                    return new Instruction.Daa();
                }
                case 0x28:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Sub(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x29:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Sub(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x2a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x2b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Sub(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x2c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Sub(Registers.Al, op1);
                }
                case 0x2d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Sub(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x2f:
                {
                    length = this.Commit();
                    return new Instruction.Das();
                }
                case 0x30:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Xor(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x31:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Xor(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x32:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x33:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Xor(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x34:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Xor(Registers.Al, op1);
                }
                case 0x35:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Xor(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x37:
                {
                    length = this.Commit();
                    return new Instruction.Aaa();
                }
                case 0x38:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Cmp(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x39:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Cmp(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x3a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x3b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Cmp(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x3c:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Cmp(Registers.Al, op1);
                }
                case 0x3d:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Cmp(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
                }
                case 0x3f:
                {
                    length = this.Commit();
                    return new Instruction.Aas();
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
                    return new Instruction.Inc(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
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
                    return new Instruction.Dec(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
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
                    return new Instruction.Push(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
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
                    return new Instruction.Pop(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                }
                case 0x68:
                {
                    op0 = this.ParseImmediate(DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Push(op0);
                }
                case 0x69:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op2 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1, op2);
                    }
                    break;
                }
                case 0x6a:
                {
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Push(op0);
                }
                case 0x6b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        op2 = this.ParseImmediate(DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Imul(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1, op2);
                    }
                    break;
                }
                case 0x70:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jo(op0);
                }
                case 0x71:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jno(op0);
                }
                case 0x72:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jb(op0);
                }
                case 0x73:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jae(op0);
                }
                case 0x74:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Je(op0);
                }
                case 0x75:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jne(op0);
                }
                case 0x76:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jbe(op0);
                }
                case 0x77:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Ja(op0);
                }
                case 0x78:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Js(op0);
                }
                case 0x79:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jns(op0);
                }
                case 0x7a:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jp(op0);
                }
                case 0x7b:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jnp(op0);
                }
                case 0x7c:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jl(op0);
                }
                case 0x7d:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jge(op0);
                }
                case 0x7e:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jle(op0);
                }
                case 0x7f:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jg(op0);
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
                            return new Instruction.Add(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instruction.Or(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instruction.Adc(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instruction.Sbb(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instruction.And(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instruction.Sub(op0, op1);
                        }
                        case 0x06:
                        {
                            return new Instruction.Xor(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instruction.Cmp(op0, op1);
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
                            return new Instruction.Add(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instruction.Or(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instruction.Adc(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instruction.Sbb(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instruction.And(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instruction.Sub(op0, op1);
                        }
                        case 0x06:
                        {
                            return new Instruction.Xor(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instruction.Cmp(op0, op1);
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
                            return new Instruction.Add(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instruction.Or(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instruction.Adc(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instruction.Sbb(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instruction.And(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instruction.Sub(op0, op1);
                        }
                        case 0x06:
                        {
                            return new Instruction.Xor(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instruction.Cmp(op0, op1);
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
                        return new Instruction.Test(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x85:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Test(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x86:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Xchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x87:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Xchg(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Xchg(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x88:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Mov(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte));
                    }
                    break;
                }
                case 0x89:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Mov(op0, FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                    }
                    break;
                }
                case 0x8a:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, DataWidth.Byte);
                        length = this.Commit();
                        return new Instruction.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, DataWidth.Byte), op1);
                    }
                    break;
                }
                case 0x8b:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                        length = this.Commit();
                        return new Instruction.Mov(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
                    }
                    break;
                }
                case 0x8d:
                {
                    if (this.TryParseByte(out modrm_byte))
                    {
                        op1 = this.ParseRM(modrm_byte, null);
                        length = this.Commit();
                        return new Instruction.Lea(FromRegisterIndex((modrm_byte >> 3) & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
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
                            return new Instruction.Pop(op0);
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
                        return new Instruction.Xchg(Registers.Ax, FromRegisterIndex(byte0 & 0b111, DataWidth.Word));
                    }
                    if (HasPrefix(0xf3))
                    {
                        length = this.Commit();
                        return new Instruction.Pause();
                    }
                    length = this.Commit();
                    return new Instruction.Nop();
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
                    return new Instruction.Xchg(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword));
                }
                case 0x98:
                {
                    if (HasPrefix(0x66))
                    {
                        length = this.Commit();
                        return new Instruction.Cbw();
                    }
                    length = this.Commit();
                    return new Instruction.Cwde();
                }
                case 0x99:
                {
                    if (HasPrefix(0x66))
                    {
                        length = this.Commit();
                        return new Instruction.Cwd();
                    }
                    length = this.Commit();
                    return new Instruction.Cdq();
                }
                case 0x9e:
                {
                    length = this.Commit();
                    return new Instruction.Sahf();
                }
                case 0x9f:
                {
                    length = this.Commit();
                    return new Instruction.Lahf();
                }
                case 0xa8:
                {
                    op1 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Test(Registers.Al, op1);
                }
                case 0xa9:
                {
                    op1 = this.ParseImmediate(HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Test(HasPrefix(0x66) ? Registers.Ax : Registers.Eax, op1);
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
                    return new Instruction.Mov(FromRegisterIndex(byte0 & 0b111, DataWidth.Byte), op1);
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
                    return new Instruction.Mov(FromRegisterIndex(byte0 & 0b111, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword), op1);
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
                            return new Instruction.Rol(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instruction.Ror(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instruction.Rcl(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instruction.Rcr(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instruction.Sal(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instruction.Shr(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instruction.Sar(op0, op1);
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
                            return new Instruction.Rol(op0, op1);
                        }
                        case 0x01:
                        {
                            return new Instruction.Ror(op0, op1);
                        }
                        case 0x02:
                        {
                            return new Instruction.Rcl(op0, op1);
                        }
                        case 0x03:
                        {
                            return new Instruction.Rcr(op0, op1);
                        }
                        case 0x04:
                        {
                            return new Instruction.Sal(op0, op1);
                        }
                        case 0x05:
                        {
                            return new Instruction.Shr(op0, op1);
                        }
                        case 0x07:
                        {
                            return new Instruction.Sar(op0, op1);
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
                    return new Instruction.Ret(op0);
                }
                case 0xc3:
                {
                    length = this.Commit();
                    return new Instruction.Ret();
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
                            return new Instruction.Mov(op0, op1);
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
                            return new Instruction.Mov(op0, op1);
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
                    return new Instruction.Enter(op0, op1);
                }
                case 0xc9:
                {
                    length = this.Commit();
                    return new Instruction.Leave();
                }
                case 0xcc:
                {
                    length = this.Commit();
                    return new Instruction.Int(new Constant(3));
                }
                case 0xcd:
                {
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Int(op0);
                }
                case 0xce:
                {
                    length = this.Commit();
                    return new Instruction.Into();
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
                            return new Instruction.Rol(op0, new Constant(1));
                        }
                        case 0x01:
                        {
                            return new Instruction.Ror(op0, new Constant(1));
                        }
                        case 0x02:
                        {
                            return new Instruction.Rcl(op0, new Constant(1));
                        }
                        case 0x03:
                        {
                            return new Instruction.Rcr(op0, new Constant(1));
                        }
                        case 0x04:
                        {
                            return new Instruction.Sal(op0, new Constant(1));
                        }
                        case 0x05:
                        {
                            return new Instruction.Shr(op0, new Constant(1));
                        }
                        case 0x07:
                        {
                            return new Instruction.Sar(op0, new Constant(1));
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
                            return new Instruction.Rol(op0, new Constant(1));
                        }
                        case 0x01:
                        {
                            return new Instruction.Ror(op0, new Constant(1));
                        }
                        case 0x02:
                        {
                            return new Instruction.Rcl(op0, new Constant(1));
                        }
                        case 0x03:
                        {
                            return new Instruction.Rcr(op0, new Constant(1));
                        }
                        case 0x04:
                        {
                            return new Instruction.Sal(op0, new Constant(1));
                        }
                        case 0x05:
                        {
                            return new Instruction.Shr(op0, new Constant(1));
                        }
                        case 0x07:
                        {
                            return new Instruction.Sar(op0, new Constant(1));
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
                            return new Instruction.Rol(op0, Registers.Cl);
                        }
                        case 0x01:
                        {
                            return new Instruction.Ror(op0, Registers.Cl);
                        }
                        case 0x02:
                        {
                            return new Instruction.Rcl(op0, Registers.Cl);
                        }
                        case 0x03:
                        {
                            return new Instruction.Rcr(op0, Registers.Cl);
                        }
                        case 0x04:
                        {
                            return new Instruction.Sal(op0, Registers.Cl);
                        }
                        case 0x05:
                        {
                            return new Instruction.Shr(op0, Registers.Cl);
                        }
                        case 0x07:
                        {
                            return new Instruction.Sar(op0, Registers.Cl);
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
                            return new Instruction.Rol(op0, Registers.Cl);
                        }
                        case 0x01:
                        {
                            return new Instruction.Ror(op0, Registers.Cl);
                        }
                        case 0x02:
                        {
                            return new Instruction.Rcl(op0, Registers.Cl);
                        }
                        case 0x03:
                        {
                            return new Instruction.Rcr(op0, Registers.Cl);
                        }
                        case 0x04:
                        {
                            return new Instruction.Sal(op0, Registers.Cl);
                        }
                        case 0x05:
                        {
                            return new Instruction.Shr(op0, Registers.Cl);
                        }
                        case 0x07:
                        {
                            return new Instruction.Sar(op0, Registers.Cl);
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
                            return new Instruction.Aam();
                        }
                        }
                        this.UnparseByte();
                    }
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Aam(op0);
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
                            return new Instruction.Aad();
                        }
                        }
                        this.UnparseByte();
                    }
                    op0 = this.ParseImmediate(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Aad(op0);
                }
                case 0xd7:
                {
                    length = this.Commit();
                    return new Instruction.Xlatb();
                }
                case 0xe3:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jecxz(op0);
                }
                case 0xe8:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Call(op0);
                }
                case 0xe9:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Dword);
                    length = this.Commit();
                    return new Instruction.Jmp(op0);
                }
                case 0xeb:
                {
                    op0 = this.ParseCodeOffset(DataWidth.Byte);
                    length = this.Commit();
                    return new Instruction.Jmp(op0);
                }
                case 0xf5:
                {
                    length = this.Commit();
                    return new Instruction.Cmc();
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
                            return new Instruction.Test(op0, op1);
                        }
                        case 0x02:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instruction.Not(op0);
                        }
                        case 0x03:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instruction.Neg(op0);
                        }
                        case 0x04:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instruction.Mul(op0);
                        }
                        case 0x05:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instruction.Imul(op0);
                        }
                        case 0x06:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instruction.Div(op0);
                        }
                        case 0x07:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Byte);
                            length = this.Commit();
                            return new Instruction.Idiv(op0);
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
                            return new Instruction.Test(op0, op1);
                        }
                        case 0x02:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Not(op0);
                        }
                        case 0x03:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Neg(op0);
                        }
                        case 0x04:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Mul(op0);
                        }
                        case 0x05:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Imul(op0);
                        }
                        case 0x06:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Div(op0);
                        }
                        case 0x07:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Idiv(op0);
                        }
                        }
                        this.UnparseByte();
                    }
                    break;
                }
                case 0xf8:
                {
                    length = this.Commit();
                    return new Instruction.Clc();
                }
                case 0xf9:
                {
                    length = this.Commit();
                    return new Instruction.Stc();
                }
                case 0xfc:
                {
                    length = this.Commit();
                    return new Instruction.Cld();
                }
                case 0xfd:
                {
                    length = this.Commit();
                    return new Instruction.Std();
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
                            return new Instruction.Inc(op0);
                        }
                        case 0x01:
                        {
                            return new Instruction.Dec(op0);
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
                            return new Instruction.Inc(op0);
                        }
                        case 0x01:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Dec(op0);
                        }
                        case 0x02:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Call(op0);
                        }
                        case 0x04:
                        {
                            op0 = this.ParseRM(modrm_byte, DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Jmp(op0);
                        }
                        case 0x06:
                        {
                            op0 = this.ParseRM(modrm_byte, HasPrefix(0x66) ? DataWidth.Word : DataWidth.Dword);
                            length = this.Commit();
                            return new Instruction.Push(op0);
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
