// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yoakke.X86.Operands;

namespace Yoakke.X86
{
    /// <summary>
    /// A builder for an x86 <see cref="Assembly"/>.
    /// </summary>
    public class AssemblyBuilder
    {
        // TODO: Sections?
        private readonly List<ICodeElement> elements = new();
        private int position;

        /// <summary>
        /// Returns a new <see cref="X86.Assembly"/> constructed from the contents of this <see cref="AssemblyBuilder"/>.
        /// </summary>
        public Assembly Assembly => new()
        {
            Elements = this.elements.ToList(),
        };

        /// <summary>
        /// The current position of the builder.
        /// </summary>
        public int Position
        {
            get => this.position;
            set
            {
                if (value < 0 || value > this.elements.Count) throw new ArgumentOutOfRangeException(nameof(value));
                this.position = value;
            }
        }

        /// <summary>
        /// Sets the current position of the builder to the given value.
        /// </summary>
        /// <param name="offset">The offset of the position to seek to relative to <paramref name="origin"/>.</param>
        /// <param name="origin">The <see cref="SeekOrigin"/> to seek relative to.</param>
        /// <returns>The new position of the builder.</returns>
        public int Seek(int offset, SeekOrigin origin)
        {
            this.Position = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => this.Position + offset,
                SeekOrigin.End => this.Position - offset,
                _ => throw new ArgumentException($"invalid {nameof(SeekOrigin)}", nameof(origin)),
            };
            return this.Position;
        }

        /// <summary>
        /// Writes an <see cref="ICodeElement"/> to the current <see cref="Position"/>.
        /// </summary>
        /// <param name="element">The <see cref="ICodeElement"/> to write.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Write(ICodeElement element)
        {
            this.elements.Insert(this.position++, element);
            return this;
        }

        /* Adding a label */

        /// <summary>
        /// Adds a new <see cref="X86.Label"/> with the given name to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="name">The name for the <see cref="X86.Label"/> to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(string name) => this.Label(new Label(name));

        /// <summary>
        /// Adds a new <see cref="X86.Label"/> with the given name to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="name">The name for the <see cref="X86.Label"/> to add.</param>
        /// <param name="labelReference">A <see cref="Operands.LabelRef"/> gets written here, that references
        /// the created label, so you can use it as an operand target later.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(string name, out LabelRef labelReference) =>
            this.Label(new Label(name), out labelReference);

        /// <summary>
        /// Adds a <see cref="X86.Label"/> to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="label">The <see cref="X86.Label"/> to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(Label label) => this.Label(label, out var _);

        /// <summary>
        /// Adds a <see cref="X86.Label"/> to the code, which can be used as a jump target.
        /// </summary>
        /// <param name="label">The <see cref="X86.Label"/> to add.</param>
        /// <param name="labelReference">A <see cref="Operands.LabelRef"/> gets written here, that references
        /// <paramref name="label"/>, so you can use it as an operand target later.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Label(Label label, out LabelRef labelReference)
        {
            labelReference = new(label);
            return this.Write(label);
        }

        /* Instructions */

        #region Generated

        /// <summary>
        /// Writes a new AAA instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Aaa(string? comment = null) =>
            this.Write(new Instruction.Aaa(comment));

        /// <summary>
        /// Writes a new AAD instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Aad(string? comment = null) =>
            this.Write(new Instruction.Aad(comment));

        /// <summary>
        /// Writes a new AAD instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Aad(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Aad(operand, comment));

        /// <summary>
        /// Writes a new AAM instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Aam(string? comment = null) =>
            this.Write(new Instruction.Aam(comment));

        /// <summary>
        /// Writes a new AAM instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Aam(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Aam(operand, comment));

        /// <summary>
        /// Writes a new AAS instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Aas(string? comment = null) =>
            this.Write(new Instruction.Aas(comment));

        /// <summary>
        /// Writes a new ADC instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Adc(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Adc(target, source, comment));

        /// <summary>
        /// Writes a new ADCX instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Adcx(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Adcx(target, source, comment));

        /// <summary>
        /// Writes a new ADD instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Add(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Add(target, source, comment));

        /// <summary>
        /// Writes a new ADOX instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Adox(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Adox(target, source, comment));

        /// <summary>
        /// Writes a new AND instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder And(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.And(target, source, comment));

        /// <summary>
        /// Writes a new ANDN instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Andn(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Andn(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new BEXTR instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bextr(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Bextr(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new BLCFILL instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blcfill(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blcfill(destination, source, comment));

        /// <summary>
        /// Writes a new BLCI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blci(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blci(destination, source, comment));

        /// <summary>
        /// Writes a new BLCIC instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blcic(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blcic(destination, source, comment));

        /// <summary>
        /// Writes a new BLCMSK instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blcmsk(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blcmsk(destination, source, comment));

        /// <summary>
        /// Writes a new BLCS instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blcs(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blcs(destination, source, comment));

        /// <summary>
        /// Writes a new BLSFILL instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blsfill(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blsfill(destination, source, comment));

        /// <summary>
        /// Writes a new BLSI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blsi(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blsi(destination, source, comment));

        /// <summary>
        /// Writes a new BLSIC instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blsic(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blsic(destination, source, comment));

        /// <summary>
        /// Writes a new BLSMSK instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blsmsk(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blsmsk(destination, source, comment));

        /// <summary>
        /// Writes a new BLSR instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Blsr(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Blsr(destination, source, comment));

        /// <summary>
        /// Writes a new BSF instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bsf(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Bsf(target, source, comment));

        /// <summary>
        /// Writes a new BSR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bsr(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Bsr(target, source, comment));

        /// <summary>
        /// Writes a new BSWAP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bswap(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Bswap(operand, comment));

        /// <summary>
        /// Writes a new BT instruction to the underlying assembly.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bt(IOperand first, IOperand second, string? comment = null) =>
            this.Write(new Instruction.Bt(first, second, comment));

        /// <summary>
        /// Writes a new BTC instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Btc(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Btc(target, source, comment));

        /// <summary>
        /// Writes a new BTR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Btr(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Btr(target, source, comment));

        /// <summary>
        /// Writes a new BTS instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bts(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Bts(target, source, comment));

        /// <summary>
        /// Writes a new BZHI instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Bzhi(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Bzhi(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new CALL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Call(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Call(operand, comment));

        /// <summary>
        /// Writes a new CBW instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cbw(string? comment = null) =>
            this.Write(new Instruction.Cbw(comment));

        /// <summary>
        /// Writes a new CDQ instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cdq(string? comment = null) =>
            this.Write(new Instruction.Cdq(comment));

        /// <summary>
        /// Writes a new CLC instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Clc(string? comment = null) =>
            this.Write(new Instruction.Clc(comment));

        /// <summary>
        /// Writes a new CLD instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cld(string? comment = null) =>
            this.Write(new Instruction.Cld(comment));

        /// <summary>
        /// Writes a new CLFLUSH instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Clflush(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Clflush(operand, comment));

        /// <summary>
        /// Writes a new CLFLUSHOPT instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Clflushopt(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Clflushopt(operand, comment));

        /// <summary>
        /// Writes a new CLWB instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Clwb(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Clwb(operand, comment));

        /// <summary>
        /// Writes a new CLZERO instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Clzero(string? comment = null) =>
            this.Write(new Instruction.Clzero(comment));

        /// <summary>
        /// Writes a new CMC instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmc(string? comment = null) =>
            this.Write(new Instruction.Cmc(comment));

        /// <summary>
        /// Writes a new CMOVA instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmova(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmova(target, source, comment));

        /// <summary>
        /// Writes a new CMOVAE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovae(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovae(target, source, comment));

        /// <summary>
        /// Writes a new CMOVB instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovb(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovb(target, source, comment));

        /// <summary>
        /// Writes a new CMOVBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovbe(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovbe(target, source, comment));

        /// <summary>
        /// Writes a new CMOVC instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovc(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovc(target, source, comment));

        /// <summary>
        /// Writes a new CMOVE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmove(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmove(target, source, comment));

        /// <summary>
        /// Writes a new CMOVG instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovg(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovg(target, source, comment));

        /// <summary>
        /// Writes a new CMOVGE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovge(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovge(target, source, comment));

        /// <summary>
        /// Writes a new CMOVL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovl(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovl(target, source, comment));

        /// <summary>
        /// Writes a new CMOVLE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovle(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovle(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNA instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovna(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovna(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNAE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnae(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnae(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNB instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnb(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnb(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnbe(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnbe(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNC instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnc(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnc(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovne(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovne(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNG instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovng(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovng(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNGE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnge(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnge(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnl(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnl(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNLE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnle(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnle(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNO instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovno(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovno(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNP instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnp(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnp(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNS instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovns(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovns(target, source, comment));

        /// <summary>
        /// Writes a new CMOVNZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovnz(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovnz(target, source, comment));

        /// <summary>
        /// Writes a new CMOVO instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovo(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovo(target, source, comment));

        /// <summary>
        /// Writes a new CMOVP instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovp(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovp(target, source, comment));

        /// <summary>
        /// Writes a new CMOVPE instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovpe(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovpe(target, source, comment));

        /// <summary>
        /// Writes a new CMOVPO instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovpo(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovpo(target, source, comment));

        /// <summary>
        /// Writes a new CMOVS instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovs(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovs(target, source, comment));

        /// <summary>
        /// Writes a new CMOVZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmovz(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmovz(target, source, comment));

        /// <summary>
        /// Writes a new CMP instruction to the underlying assembly.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmp(IOperand first, IOperand second, string? comment = null) =>
            this.Write(new Instruction.Cmp(first, second, comment));

        /// <summary>
        /// Writes a new CMPXCHG instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmpxchg(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cmpxchg(target, source, comment));

        /// <summary>
        /// Writes a new CMPXCHG8B instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cmpxchg8b(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Cmpxchg8b(operand, comment));

        /// <summary>
        /// Writes a new CPUID instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cpuid(string? comment = null) =>
            this.Write(new Instruction.Cpuid(comment));

        /// <summary>
        /// Writes a new CRC32 instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Crc32(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Crc32(target, source, comment));

        /// <summary>
        /// Writes a new CVTSD2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cvtsd2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cvtsd2si(destination, source, comment));

        /// <summary>
        /// Writes a new CVTSS2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cvtss2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cvtss2si(destination, source, comment));

        /// <summary>
        /// Writes a new CVTTSD2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cvttsd2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cvttsd2si(destination, source, comment));

        /// <summary>
        /// Writes a new CVTTSS2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cvttss2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Cvttss2si(destination, source, comment));

        /// <summary>
        /// Writes a new CWD instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cwd(string? comment = null) =>
            this.Write(new Instruction.Cwd(comment));

        /// <summary>
        /// Writes a new CWDE instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Cwde(string? comment = null) =>
            this.Write(new Instruction.Cwde(comment));

        /// <summary>
        /// Writes a new DAA instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Daa(string? comment = null) =>
            this.Write(new Instruction.Daa(comment));

        /// <summary>
        /// Writes a new DAS instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Das(string? comment = null) =>
            this.Write(new Instruction.Das(comment));

        /// <summary>
        /// Writes a new DEC instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Dec(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Dec(operand, comment));

        /// <summary>
        /// Writes a new DIV instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Div(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Div(operand, comment));

        /// <summary>
        /// Writes a new EMMS instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Emms(string? comment = null) =>
            this.Write(new Instruction.Emms(comment));

        /// <summary>
        /// Writes a new ENTER instruction to the underlying assembly.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Enter(IOperand first, IOperand second, string? comment = null) =>
            this.Write(new Instruction.Enter(first, second, comment));

        /// <summary>
        /// Writes a new FEMMS instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Femms(string? comment = null) =>
            this.Write(new Instruction.Femms(comment));

        /// <summary>
        /// Writes a new IDIV instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Idiv(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Idiv(operand, comment));

        /// <summary>
        /// Writes a new IMUL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Imul(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Imul(operand, comment));

        /// <summary>
        /// Writes a new IMUL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Imul(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Imul(target, source, comment));

        /// <summary>
        /// Writes a new IMUL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Imul(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Imul(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new INC instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Inc(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Inc(operand, comment));

        /// <summary>
        /// Writes a new INT instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Int(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Int(operand, comment));

        /// <summary>
        /// Writes a new INTO instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Into(string? comment = null) =>
            this.Write(new Instruction.Into(comment));

        /// <summary>
        /// Writes a new JA instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ja(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Ja(operand, comment));

        /// <summary>
        /// Writes a new JAE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jae(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jae(operand, comment));

        /// <summary>
        /// Writes a new JB instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jb(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jb(operand, comment));

        /// <summary>
        /// Writes a new JBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jbe(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jbe(operand, comment));

        /// <summary>
        /// Writes a new JC instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jc(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jc(operand, comment));

        /// <summary>
        /// Writes a new JE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Je(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Je(operand, comment));

        /// <summary>
        /// Writes a new JECXZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jecxz(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jecxz(operand, comment));

        /// <summary>
        /// Writes a new JG instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jg(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jg(operand, comment));

        /// <summary>
        /// Writes a new JGE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jge(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jge(operand, comment));

        /// <summary>
        /// Writes a new JL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jl(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jl(operand, comment));

        /// <summary>
        /// Writes a new JLE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jle(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jle(operand, comment));

        /// <summary>
        /// Writes a new JMP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jmp(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jmp(operand, comment));

        /// <summary>
        /// Writes a new JNA instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jna(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jna(operand, comment));

        /// <summary>
        /// Writes a new JNAE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnae(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnae(operand, comment));

        /// <summary>
        /// Writes a new JNB instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnb(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnb(operand, comment));

        /// <summary>
        /// Writes a new JNBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnbe(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnbe(operand, comment));

        /// <summary>
        /// Writes a new JNC instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnc(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnc(operand, comment));

        /// <summary>
        /// Writes a new JNE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jne(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jne(operand, comment));

        /// <summary>
        /// Writes a new JNG instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jng(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jng(operand, comment));

        /// <summary>
        /// Writes a new JNGE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnge(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnge(operand, comment));

        /// <summary>
        /// Writes a new JNL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnl(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnl(operand, comment));

        /// <summary>
        /// Writes a new JNLE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnle(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnle(operand, comment));

        /// <summary>
        /// Writes a new JNO instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jno(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jno(operand, comment));

        /// <summary>
        /// Writes a new JNP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnp(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnp(operand, comment));

        /// <summary>
        /// Writes a new JNS instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jns(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jns(operand, comment));

        /// <summary>
        /// Writes a new JNZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jnz(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jnz(operand, comment));

        /// <summary>
        /// Writes a new JO instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jo(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jo(operand, comment));

        /// <summary>
        /// Writes a new JP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jp(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jp(operand, comment));

        /// <summary>
        /// Writes a new JPE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jpe(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jpe(operand, comment));

        /// <summary>
        /// Writes a new JPO instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jpo(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jpo(operand, comment));

        /// <summary>
        /// Writes a new JS instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Js(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Js(operand, comment));

        /// <summary>
        /// Writes a new JZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Jz(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Jz(operand, comment));

        /// <summary>
        /// Writes a new LAHF instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Lahf(string? comment = null) =>
            this.Write(new Instruction.Lahf(comment));

        /// <summary>
        /// Writes a new LDMXCSR instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ldmxcsr(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Ldmxcsr(operand, comment));

        /// <summary>
        /// Writes a new LEA instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Lea(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Lea(destination, source, comment));

        /// <summary>
        /// Writes a new LEAVE instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Leave(string? comment = null) =>
            this.Write(new Instruction.Leave(comment));

        /// <summary>
        /// Writes a new LFENCE instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Lfence(string? comment = null) =>
            this.Write(new Instruction.Lfence(comment));

        /// <summary>
        /// Writes a new LZCNT instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Lzcnt(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Lzcnt(destination, source, comment));

        /// <summary>
        /// Writes a new MFENCE instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mfence(string? comment = null) =>
            this.Write(new Instruction.Mfence(comment));

        /// <summary>
        /// Writes a new MONITOR instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Monitor(string? comment = null) =>
            this.Write(new Instruction.Monitor(comment));

        /// <summary>
        /// Writes a new MONITORX instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Monitorx(string? comment = null) =>
            this.Write(new Instruction.Monitorx(comment));

        /// <summary>
        /// Writes a new MOV instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mov(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Mov(destination, source, comment));

        /// <summary>
        /// Writes a new MOVBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Movbe(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Movbe(destination, source, comment));

        /// <summary>
        /// Writes a new MOVNTI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Movnti(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Movnti(destination, source, comment));

        /// <summary>
        /// Writes a new MOVSX instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Movsx(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Movsx(destination, source, comment));

        /// <summary>
        /// Writes a new MOVZX instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Movzx(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Movzx(destination, source, comment));

        /// <summary>
        /// Writes a new MUL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mul(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Mul(operand, comment));

        /// <summary>
        /// Writes a new MULX instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mulx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Mulx(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new MWAIT instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mwait(string? comment = null) =>
            this.Write(new Instruction.Mwait(comment));

        /// <summary>
        /// Writes a new MWAITX instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Mwaitx(string? comment = null) =>
            this.Write(new Instruction.Mwaitx(comment));

        /// <summary>
        /// Writes a new NEG instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Neg(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Neg(operand, comment));

        /// <summary>
        /// Writes a new NOP instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Nop(string? comment = null) =>
            this.Write(new Instruction.Nop(comment));

        /// <summary>
        /// Writes a new NOT instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Not(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Not(operand, comment));

        /// <summary>
        /// Writes a new OR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Or(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Or(target, source, comment));

        /// <summary>
        /// Writes a new PAUSE instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Pause(string? comment = null) =>
            this.Write(new Instruction.Pause(comment));

        /// <summary>
        /// Writes a new PDEP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Pdep(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Pdep(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new PEXT instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Pext(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Pext(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new POP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Pop(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Pop(operand, comment));

        /// <summary>
        /// Writes a new POPCNT instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Popcnt(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Popcnt(destination, source, comment));

        /// <summary>
        /// Writes a new PREFETCH instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetch(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetch(operand, comment));

        /// <summary>
        /// Writes a new PREFETCHNTA instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetchnta(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetchnta(operand, comment));

        /// <summary>
        /// Writes a new PREFETCHT0 instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetcht0(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetcht0(operand, comment));

        /// <summary>
        /// Writes a new PREFETCHT1 instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetcht1(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetcht1(operand, comment));

        /// <summary>
        /// Writes a new PREFETCHT2 instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetcht2(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetcht2(operand, comment));

        /// <summary>
        /// Writes a new PREFETCHW instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetchw(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetchw(operand, comment));

        /// <summary>
        /// Writes a new PREFETCHWT1 instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Prefetchwt1(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Prefetchwt1(operand, comment));

        /// <summary>
        /// Writes a new PUSH instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Push(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Push(operand, comment));

        /// <summary>
        /// Writes a new RCL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rcl(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Rcl(target, source, comment));

        /// <summary>
        /// Writes a new RCR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rcr(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Rcr(target, source, comment));

        /// <summary>
        /// Writes a new RDRAND instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rdrand(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Rdrand(operand, comment));

        /// <summary>
        /// Writes a new RDSEED instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rdseed(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Rdseed(operand, comment));

        /// <summary>
        /// Writes a new RDTSC instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rdtsc(string? comment = null) =>
            this.Write(new Instruction.Rdtsc(comment));

        /// <summary>
        /// Writes a new RDTSCP instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rdtscp(string? comment = null) =>
            this.Write(new Instruction.Rdtscp(comment));

        /// <summary>
        /// Writes a new RET instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ret(string? comment = null) =>
            this.Write(new Instruction.Ret(comment));

        /// <summary>
        /// Writes a new RET instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ret(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Ret(operand, comment));

        /// <summary>
        /// Writes a new ROL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rol(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Rol(target, source, comment));

        /// <summary>
        /// Writes a new ROR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ror(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Ror(target, source, comment));

        /// <summary>
        /// Writes a new RORX instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Rorx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Rorx(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new SAHF instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sahf(string? comment = null) =>
            this.Write(new Instruction.Sahf(comment));

        /// <summary>
        /// Writes a new SAL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sal(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Sal(target, source, comment));

        /// <summary>
        /// Writes a new SAR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sar(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Sar(target, source, comment));

        /// <summary>
        /// Writes a new SARX instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sarx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Sarx(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new SBB instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sbb(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Sbb(target, source, comment));

        /// <summary>
        /// Writes a new SETA instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Seta(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Seta(operand, comment));

        /// <summary>
        /// Writes a new SETAE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setae(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setae(operand, comment));

        /// <summary>
        /// Writes a new SETB instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setb(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setb(operand, comment));

        /// <summary>
        /// Writes a new SETBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setbe(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setbe(operand, comment));

        /// <summary>
        /// Writes a new SETC instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setc(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setc(operand, comment));

        /// <summary>
        /// Writes a new SETE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sete(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Sete(operand, comment));

        /// <summary>
        /// Writes a new SETG instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setg(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setg(operand, comment));

        /// <summary>
        /// Writes a new SETGE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setge(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setge(operand, comment));

        /// <summary>
        /// Writes a new SETL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setl(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setl(operand, comment));

        /// <summary>
        /// Writes a new SETLE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setle(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setle(operand, comment));

        /// <summary>
        /// Writes a new SETNA instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setna(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setna(operand, comment));

        /// <summary>
        /// Writes a new SETNAE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnae(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnae(operand, comment));

        /// <summary>
        /// Writes a new SETNB instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnb(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnb(operand, comment));

        /// <summary>
        /// Writes a new SETNBE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnbe(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnbe(operand, comment));

        /// <summary>
        /// Writes a new SETNC instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnc(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnc(operand, comment));

        /// <summary>
        /// Writes a new SETNE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setne(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setne(operand, comment));

        /// <summary>
        /// Writes a new SETNG instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setng(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setng(operand, comment));

        /// <summary>
        /// Writes a new SETNGE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnge(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnge(operand, comment));

        /// <summary>
        /// Writes a new SETNL instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnl(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnl(operand, comment));

        /// <summary>
        /// Writes a new SETNLE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnle(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnle(operand, comment));

        /// <summary>
        /// Writes a new SETNO instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setno(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setno(operand, comment));

        /// <summary>
        /// Writes a new SETNP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnp(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnp(operand, comment));

        /// <summary>
        /// Writes a new SETNS instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setns(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setns(operand, comment));

        /// <summary>
        /// Writes a new SETNZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setnz(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setnz(operand, comment));

        /// <summary>
        /// Writes a new SETO instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Seto(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Seto(operand, comment));

        /// <summary>
        /// Writes a new SETP instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setp(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setp(operand, comment));

        /// <summary>
        /// Writes a new SETPE instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setpe(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setpe(operand, comment));

        /// <summary>
        /// Writes a new SETPO instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setpo(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setpo(operand, comment));

        /// <summary>
        /// Writes a new SETS instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sets(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Sets(operand, comment));

        /// <summary>
        /// Writes a new SETZ instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Setz(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Setz(operand, comment));

        /// <summary>
        /// Writes a new SFENCE instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sfence(string? comment = null) =>
            this.Write(new Instruction.Sfence(comment));

        /// <summary>
        /// Writes a new SHL instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Shl(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Shl(target, source, comment));

        /// <summary>
        /// Writes a new SHLD instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Shld(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Shld(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new SHLX instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Shlx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Shlx(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new SHR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Shr(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Shr(target, source, comment));

        /// <summary>
        /// Writes a new SHRD instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Shrd(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Shrd(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new SHRX instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand1">The 1st operand.</param>
        /// <param name="operand2">The 2nd operand.</param>
        /// <param name="operand3">The 3rd operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Shrx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null) =>
            this.Write(new Instruction.Shrx(operand1, operand2, operand3, comment));

        /// <summary>
        /// Writes a new STC instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Stc(string? comment = null) =>
            this.Write(new Instruction.Stc(comment));

        /// <summary>
        /// Writes a new STD instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Std(string? comment = null) =>
            this.Write(new Instruction.Std(comment));

        /// <summary>
        /// Writes a new STMXCSR instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Stmxcsr(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Stmxcsr(operand, comment));

        /// <summary>
        /// Writes a new SUB instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Sub(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Sub(target, source, comment));

        /// <summary>
        /// Writes a new T1MSKC instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder T1mskc(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.T1mskc(destination, source, comment));

        /// <summary>
        /// Writes a new TEST instruction to the underlying assembly.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Test(IOperand first, IOperand second, string? comment = null) =>
            this.Write(new Instruction.Test(first, second, comment));

        /// <summary>
        /// Writes a new TZCNT instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Tzcnt(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Tzcnt(destination, source, comment));

        /// <summary>
        /// Writes a new TZMSK instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Tzmsk(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Tzmsk(destination, source, comment));

        /// <summary>
        /// Writes a new UD2 instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ud2(string? comment = null) =>
            this.Write(new Instruction.Ud2(comment));

        /// <summary>
        /// Writes a new VCVTSD2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvtsd2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvtsd2si(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTSD2USI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvtsd2usi(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvtsd2usi(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTSS2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvtss2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvtss2si(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTSS2USI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvtss2usi(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvtss2usi(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTTSD2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvttsd2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvttsd2si(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTTSD2USI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvttsd2usi(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvttsd2usi(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTTSS2SI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvttss2si(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvttss2si(destination, source, comment));

        /// <summary>
        /// Writes a new VCVTTSS2USI instruction to the underlying assembly.
        /// </summary>
        /// <param name="destination">The output operand.</param>
        /// <param name="source">The input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vcvttss2usi(IOperand destination, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Vcvttss2usi(destination, source, comment));

        /// <summary>
        /// Writes a new VLDMXCSR instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vldmxcsr(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Vldmxcsr(operand, comment));

        /// <summary>
        /// Writes a new VSTMXCSR instruction to the underlying assembly.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vstmxcsr(IOperand operand, string? comment = null) =>
            this.Write(new Instruction.Vstmxcsr(operand, comment));

        /// <summary>
        /// Writes a new VZEROALL instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vzeroall(string? comment = null) =>
            this.Write(new Instruction.Vzeroall(comment));

        /// <summary>
        /// Writes a new VZEROUPPER instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Vzeroupper(string? comment = null) =>
            this.Write(new Instruction.Vzeroupper(comment));

        /// <summary>
        /// Writes a new XADD instruction to the underlying assembly.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Xadd(IOperand first, IOperand second, string? comment = null) =>
            this.Write(new Instruction.Xadd(first, second, comment));

        /// <summary>
        /// Writes a new XCHG instruction to the underlying assembly.
        /// </summary>
        /// <param name="first">The first operand.</param>
        /// <param name="second">The second operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Xchg(IOperand first, IOperand second, string? comment = null) =>
            this.Write(new Instruction.Xchg(first, second, comment));

        /// <summary>
        /// Writes a new XGETBV instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Xgetbv(string? comment = null) =>
            this.Write(new Instruction.Xgetbv(comment));

        /// <summary>
        /// Writes a new XLATB instruction to the underlying assembly.
        /// </summary>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Xlatb(string? comment = null) =>
            this.Write(new Instruction.Xlatb(comment));

        /// <summary>
        /// Writes a new XOR instruction to the underlying assembly.
        /// </summary>
        /// <param name="target">The first input (and output) operand.</param>
        /// <param name="source">The second input operand.</param>
        /// <param name="comment">The optional inline comment.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Xor(IOperand target, IOperand source, string? comment = null) =>
            this.Write(new Instruction.Xor(target, source, comment));

        #endregion Generated
    }
}
