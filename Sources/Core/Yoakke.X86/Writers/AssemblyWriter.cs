// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Yoakke.X86.Operands;

namespace Yoakke.X86.Writers
{
    /// <summary>
    /// Basic class for writers that emit textual assembly code.
    /// </summary>
    public class AssemblyWriter
    {
        /// <summary>
        /// The underlying <see cref="StringBuilder"/> this <see cref="AssemblyWriter"/> writes to.
        /// </summary>
        public StringBuilder Result { get; }

        /// <summary>
        /// Settings for this <see cref="AssemblyWriter"/>.
        /// </summary>
        public AssemblyWriterSettings Settings { get; set; } = AssemblyWriterSettings.Default;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyWriter"/> class.
        /// </summary>
        /// <param name="result">The <see cref="StringBuilder"/> to write the code to.</param>
        public AssemblyWriter(StringBuilder result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyWriter"/> class.
        /// </summary>
        public AssemblyWriter()
            : this(new StringBuilder())
        {
        }

        /// <summary>
        /// Writes a character to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="c">The character to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter Write(char c)
        {
            this.Result.Append(c);
            return this;
        }

        /// <summary>
        /// Writes a string to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter Write(string str)
        {
            this.Result.Append(str);
            return this;
        }

        /// <summary>
        /// Starts a new line for the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteLine()
        {
            this.Result.AppendLine();
            return this;
        }

        /// <summary>
        /// Writes a string to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteLine(string str) => this.Write(str).WriteLine();

        /// <summary>
        /// Writes an object to the underlying <see cref="StringBuilder"/>. Handles <see cref="ICodeElement"/>s
        /// and <see cref="IOperand"/>s.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter Write(object? obj) => obj switch
        {
            IOperand op => this.Write(op),
            ICodeElement elem => this.Write(elem),
            _ => this.Write(obj?.ToString() ?? "null"),
        };

        /// <summary>
        /// Writes an object to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// Handles <see cref="ICodeElement"/>s and <see cref="IOperand"/>s.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteLine(object? obj) => this.Write(obj).WriteLine();

        /// <summary>
        /// Writes a separated sequence of objects to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <typeparam name="T">The type of the separated elements.</typeparam>
        /// <param name="separator">The separator sequence to insert between elements.</param>
        /// <param name="items">The sequence of elements to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter Write<T>(string separator, IEnumerable<T> items)
        {
            var first = true;
            foreach (var item in items)
            {
                if (!first) this.Write(separator);
                first = false;
                this.Write(item);
            }
            return this;
        }

        /// <summary>
        /// Writes a separated sequence of objects to the underlying <see cref="StringBuilder"/> and starts a new line.
        /// </summary>
        /// <typeparam name="T">The type of the separated elements.</typeparam>
        /// <param name="separator">The separator sequence to insert between elements.</param>
        /// <param name="items">The sequence of elements to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteLine<T>(string separator, IEnumerable<T> items) =>
            this.Write(separator, items).WriteLine();

        /// <summary>
        /// Writes a keyword to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="str">The keyword string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter WriteKeyword(string str) =>
            this.Write(this.Settings.KeywordsUpperCase ? str.ToUpper() : str.ToLower());

        /// <summary>
        /// Writes a keyword to the underlying <see cref="StringBuilder"/> and starts a new line.
        /// </summary>
        /// <param name="str">The keyword string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteKeywordLine(string str) => this.WriteKeyword(str).WriteLine();

        /// <summary>
        /// Writes a comment to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="str">The comment string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter WriteComment(string str)
        {
            // First we count how far off we are from the line start
            // This is so we can align other lines nicely with this one
            // We build a prefix of characters
            var linePrefix = this.GetLastLinePrefix();
            // Now write the comment line-by-line
            var reader = new StringReader(str);
            var first = true;
            while (true)
            {
                var line = reader.ReadLine();
                if (line is null) break;

                if (!first) this.WriteLine().Write(linePrefix);
                first = false;
                this.Write(this.Settings.CommentPrefix).Write(line);
            }
            return this;
        }

        /// <summary>
        /// Writes a comment to the underlying <see cref="StringBuilder"/> and starts a new line.
        /// </summary>
        /// <param name="str">The comment string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteCommentLine(string str) => this.WriteComment(str).WriteLine();

        /// <summary>
        /// Writes an <see cref="ICodeElement"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="element">The <see cref="ICodeElement"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(ICodeElement element) => element switch
        {
            IInstruction instruction => this.Write(instruction),
            Label label => this.Write(label),

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Writes an <see cref="ICodeElement"/> to the underlying <see cref="StringBuilder"/> and starts a new line.
        /// </summary>
        /// <param name="element">The <see cref="ICodeElement"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteLine(ICodeElement element) => this.Write(element).WriteLine();

        /// <summary>
        /// Writes an <see cref="IOperand"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="operand">The <see cref="IOperand"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(IOperand operand) => operand switch
        {
            Register r => this.Write(r),
            Segment s => this.Write(s),
            Address a => this.Write(a),
            FarAddress f => this.Write(f),
            Indirect i => this.Write(i),
            LabelRef l => this.Write(l),
            Constant c => this.Write(c),

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Writes an <see cref="IOperand"/> to the underlying <see cref="StringBuilder"/> and starts a new line.
        /// </summary>
        /// <param name="operand">The <see cref="IOperand"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyWriter WriteLine(IOperand operand) => this.Write(operand).WriteLine();

        /// <summary>
        /// Writes an <see cref="IInstruction"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(IInstruction instruction)
        {
            // Get the instruction name from it's type
            var ins = instruction.GetType().Name.ToString();

            if (this.Settings.SyntaxFlavor == SyntaxFlavor.ATnT)
            {
                // AT&T syntax wants a suffix to determine operand
                var operandSize = instruction.Operands.Select(op => op.GetSize()).FirstOrDefault(op => op is not null);
                if (operandSize is not null)
                {
                    var suffix = this.GetATnTSuffix(operandSize.Value);
                    ins = $"{ins}{suffix}";
                }
            }

            // Pad the name
            ins = ins.PadRight(this.Settings.InstructionPadding);

            // The instruction name is complete
            this.Write(this.Settings.InstructionsUpperCase ? ins.ToUpper() : ins.ToLower());

            // We reverse args for AT&T
            var operands = this.Settings.SyntaxFlavor == SyntaxFlavor.ATnT
                ? instruction.Operands.Reverse()
                : instruction.Operands;

            // Write operands
            var first = true;
            foreach (var op in operands)
            {
                // Separator
                if (first) this.Write(' ');
                else this.Write(", ");
                first = false;

                // Operand
                this.Write(op);
            }

            // If there is a comment, append it
            if (instruction.Comment is not null) this.Write(' ').WriteComment(instruction.Comment);

            return this;
        }

        /// <summary>
        /// Writes a <see cref="Register"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="register">The <see cref="Register"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Register register) => this.WriteRegister(register.Name);

        /// <summary>
        /// Writes a <see cref="Segment"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="segment">The <see cref="Segment"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Segment segment) => this.WriteRegister(segment.Name);

        /// <summary>
        /// Writes an <see cref="Indirect"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="indirect">The <see cref="Indirect"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Indirect indirect)
        {
            if (this.Settings.SyntaxFlavor == SyntaxFlavor.Intel)
            {
                // Intel
                // Write the size prefix
                this.WriteKeyword($"{indirect.Size} ptr").Write(' ').Write(indirect.Address);
            }
            else
            {
                // AT&T
                // Just write out the address
                this.Write(indirect.Address);
            }
            return this;
        }

        /// <summary>
        /// Writes an <see cref="Address"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="address">The <see cref="Address"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Address address)
        {
            if (this.Settings.SyntaxFlavor == SyntaxFlavor.Intel)
            {
                // Intel, means
                // segment:[base + index * scale + displacement]
                // or
                // [segment: base + index * scale + displacement]

                // Segment selector and bracket dance
                if (this.Settings.SegmentSelectorInBrackets) this.Write('[');
                if (address.Segment is not null)
                {
                    this.Write(address.Segment.Value).Write(':');
                    if (this.Settings.SegmentSelectorInBrackets) this.Write(' ');
                }
                if (!this.Settings.SegmentSelectorInBrackets) this.Write('[');

                var written = false;

                // Base address
                if (address.Base is not null)
                {
                    written = true;
                    this.Write(address.Base.Value);
                }

                // Scaled index
                if (address.ScaledIndex is not null)
                {
                    if (written) this.Write(" + ");
                    written = true;

                    var (index, scale) = address.ScaledIndex.Value;
                    this.Write(index).Write(" * ").Write(scale);
                }

                // Displacement
                if (address.DisplacementLabel is not null)
                {
                    if (written) this.Write(" + ");
                    var labelRef = address.DisplacementLabel.Value;
                    this.Write(labelRef.Label.Name);
                    var disp = labelRef.Offset + address.Displacement;
                    if (disp != 0) this.Write(disp > 0 ? " + " : " - ").Write(Math.Abs(disp));
                }
                else if (!written || address.Displacement != 0)
                {
                    if (written) this.Write(address.Displacement > 0 ? " + " : " - ");
                    this.Write(Math.Abs(address.Displacement));
                }

                this.Write(']');
            }
            else
            {
                // AT&T
                // segment:displacement(base, index, scale)
                // who came up with this???

                // Segment selector
                if (address.Segment is not null) this.Write(address.Segment.Value).Write(':');

                // Displacement
                if (address.DisplacementLabel is not null)
                {
                    var labelRef = address.DisplacementLabel.Value;
                    this.Write(labelRef.Label.Name);
                    var disp = labelRef.Offset + address.Displacement;
                    if (disp != 0) this.Write(disp > 0 ? " + " : " - ").Write(Math.Abs(disp));
                }
                else
                {
                    this.Write(address.Displacement);
                }

                this.Write('(');
                // Base
                if (address.Base is not null) this.Write(address.Base.Value);
                // Index, scale
                if (address.ScaledIndex is not null)
                {
                    this.Write(',');
                    var (index, scale) = address.ScaledIndex.Value;
                    this.Write(' ').Write(index).Write(", ").Write(scale);
                }
                else
                {
                    if (address.Base is null) this.Write(",,");
                }
                this.Write(')');
            }
            return this;
        }

        /// <summary>
        /// Writes a <see cref="FarAddress"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="farAddress">The <see cref="FarAddress"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(FarAddress farAddress) =>
            this.Write(farAddress.Segment).Write(':').Write(farAddress.Address);

        /// <summary>
        /// Writes a <see cref="Constant"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="constant">The <see cref="Constant"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Constant constant)
        {
            if (this.Settings.SyntaxFlavor == SyntaxFlavor.ATnT) this.Write('$');
            return this.Write(constant.Value.ToString() ?? string.Empty);
        }

        /// <summary>
        /// Writes a <see cref="Label"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="label">The <see cref="Label"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Label label) => this.Write(label.Name).Write(':');

        /// <summary>
        /// Writes a <see cref="LabelRef"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="label">The <see cref="LabelRef"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(LabelRef label)
        {
            this.Write(label.Label.Name);
            if (label.Offset != 0) this.Write(" + ").Write(label.Offset);
            return this;
        }

        /// <summary>
        /// Writes the whole <see cref="Assembly"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public virtual AssemblyWriter Write(Assembly assembly)
        {
            foreach (var element in assembly.Elements)
            {
                if (element is IInstruction) this.Write(this.Settings.InstructionIndentation);
                this.WriteLine(element);
            }
            return this;
        }

        private AssemblyWriter WriteRegister(string name) => this
            .Write(this.Settings.SyntaxFlavor == SyntaxFlavor.ATnT ? "%" : string.Empty)
            .Write(this.Settings.RegistersUpperCase ? name.ToUpper() : name);

        private string GetATnTSuffix(DataWidth size) => size switch
        {
            DataWidth.Byte => "b",
            DataWidth.Word => "w",
            DataWidth.Dword => "l",
            DataWidth.Qword => "q",

            _ => throw new NotSupportedException(),
        };

        private string GetLastLinePrefix()
        {
            static bool IsNewline(char ch) => ch == '\r' || ch == '\n';

            if (this.Result.Length == 0 || IsNewline(this.Result[this.Result.Length - 1])) return string.Empty;

            var result = new StringBuilder();
            var i = this.Result.Length - 1;
            for (; i >= 0; --i)
            {
                var current = this.Result[i];
                if (IsNewline(current)) break;

                if (char.IsWhiteSpace(current)) result.Insert(0, current);
                else if (!char.IsControl(current)) result.Insert(0, ' ');
            }
            return result.ToString();
        }
    }
}
