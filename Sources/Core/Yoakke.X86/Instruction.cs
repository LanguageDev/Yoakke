// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.X86;
using Yoakke.X86.Operands;

namespace Yoakke.X86
{
    /// <summary>
    /// A wrapper class for all instructions.
    /// </summary>
    public static class Instruction
    {
        #region Generated

        /// <summary>
        /// ASCII Adjust After Addition.
        /// </summary>
        public class Aaa : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Aaa"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Aaa(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// ASCII Adjust AX Before Division.
        /// </summary>
        public class Aad : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Aad"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Aad(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Aad"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Aad(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// ASCII Adjust AX After Multiply.
        /// </summary>
        public class Aam : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Aam"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Aam(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Aam"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Aam(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// ASCII Adjust AL After Subtraction.
        /// </summary>
        public class Aas : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Aas"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Aas(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Add with Carry.
        /// </summary>
        public class Adc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Adc"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Adc(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Unsigned Integer Addition of Two Operands with Carry Flag.
        /// </summary>
        public class Adcx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Adcx"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Adcx(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Add.
        /// </summary>
        public class Add : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Add"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Add(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Unsigned Integer Addition of Two Operands with Overflow Flag.
        /// </summary>
        public class Adox : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Adox"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Adox(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical AND.
        /// </summary>
        public class And : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="And"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public And(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical AND NOT.
        /// </summary>
        public class Andn : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Andn"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Andn(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Field Extract.
        /// </summary>
        public class Bextr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bextr"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bextr(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Fill From Lowest Clear Bit.
        /// </summary>
        public class Blcfill : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blcfill"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blcfill(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Isolate Lowest Clear Bit.
        /// </summary>
        public class Blci : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blci"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blci(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Isolate Lowest Set Bit and Complement.
        /// </summary>
        public class Blcic : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blcic"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blcic(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Mask From Lowest Clear Bit.
        /// </summary>
        public class Blcmsk : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blcmsk"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blcmsk(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set Lowest Clear Bit.
        /// </summary>
        public class Blcs : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blcs"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blcs(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Fill From Lowest Set Bit.
        /// </summary>
        public class Blsfill : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blsfill"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blsfill(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Isolate Lowest Set Bit.
        /// </summary>
        public class Blsi : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blsi"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blsi(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Isolate Lowest Set Bit and Complement.
        /// </summary>
        public class Blsic : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blsic"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blsic(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Mask From Lowest Set Bit.
        /// </summary>
        public class Blsmsk : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blsmsk"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blsmsk(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Reset Lowest Set Bit.
        /// </summary>
        public class Blsr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Blsr"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Blsr(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Scan Forward.
        /// </summary>
        public class Bsf : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bsf"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bsf(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Scan Reverse.
        /// </summary>
        public class Bsr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bsr"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bsr(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Byte Swap.
        /// </summary>
        public class Bswap : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bswap"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bswap(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Test.
        /// </summary>
        public class Bt : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first operand.
            /// </summary>
            public IOperand First => this.Operands[0];

            /// <summary>
            /// The second operand.
            /// </summary>
            public IOperand Second => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bt"/> class.
            /// </summary>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bt(IOperand first, IOperand second, string? comment = null)
            {
                this.Operands = new[] { first, second };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Test and Complement.
        /// </summary>
        public class Btc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Btc"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Btc(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Test and Reset.
        /// </summary>
        public class Btr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Btr"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Btr(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Bit Test and Set.
        /// </summary>
        public class Bts : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bts"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bts(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Zero High Bits Starting with Specified Bit Position.
        /// </summary>
        public class Bzhi : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Bzhi"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Bzhi(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Call Procedure.
        /// </summary>
        public class Call : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Call"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Call(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Byte to Word.
        /// </summary>
        public class Cbw : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cbw"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cbw(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Doubleword to Quadword.
        /// </summary>
        public class Cdq : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cdq"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cdq(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Clear Carry Flag.
        /// </summary>
        public class Clc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Clc"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Clc(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Clear Direction Flag.
        /// </summary>
        public class Cld : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cld"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cld(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Flush Cache Line.
        /// </summary>
        public class Clflush : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Clflush"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Clflush(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Flush Cache Line Optimized.
        /// </summary>
        public class Clflushopt : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Clflushopt"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Clflushopt(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Cache Line Write Back.
        /// </summary>
        public class Clwb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Clwb"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Clwb(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Zero-out 64-bit Cache Line.
        /// </summary>
        public class Clzero : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Clzero"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Clzero(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Complement Carry Flag.
        /// </summary>
        public class Cmc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmc"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cmc(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if above (CF == 0 and ZF == 0).
        /// </summary>
        public class Cmova : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmova"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmova(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if above or equal (CF == 0).
        /// </summary>
        public class Cmovae : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovae"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovae(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if below (CF == 1).
        /// </summary>
        public class Cmovb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovb"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovb(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if below or equal (CF == 1 or ZF == 1).
        /// </summary>
        public class Cmovbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovbe"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovbe(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if carry (CF == 1).
        /// </summary>
        public class Cmovc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovc"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovc(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if equal (ZF == 1).
        /// </summary>
        public class Cmove : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmove"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmove(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if greater (ZF == 0 and SF == OF).
        /// </summary>
        public class Cmovg : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovg"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovg(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if greater or equal (SF == OF).
        /// </summary>
        public class Cmovge : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovge"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovge(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if less (SF != OF).
        /// </summary>
        public class Cmovl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovl"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovl(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if less or equal (ZF == 1 or SF != OF).
        /// </summary>
        public class Cmovle : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovle"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovle(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not above (CF == 1 or ZF == 1).
        /// </summary>
        public class Cmovna : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovna"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovna(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not above or equal (CF == 1).
        /// </summary>
        public class Cmovnae : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnae"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnae(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not below (CF == 0).
        /// </summary>
        public class Cmovnb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnb"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnb(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not below or equal (CF == 0 and ZF == 0).
        /// </summary>
        public class Cmovnbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnbe"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnbe(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not carry (CF == 0).
        /// </summary>
        public class Cmovnc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnc"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnc(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not equal (ZF == 0).
        /// </summary>
        public class Cmovne : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovne"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovne(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not greater (ZF == 1 or SF != OF).
        /// </summary>
        public class Cmovng : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovng"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovng(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not greater or equal (SF != OF).
        /// </summary>
        public class Cmovnge : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnge"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnge(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not less (SF == OF).
        /// </summary>
        public class Cmovnl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnl"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnl(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not less or equal (ZF == 0 and SF == OF).
        /// </summary>
        public class Cmovnle : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnle"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnle(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not overflow (OF == 0).
        /// </summary>
        public class Cmovno : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovno"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovno(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not parity (PF == 0).
        /// </summary>
        public class Cmovnp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnp"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnp(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not sign (SF == 0).
        /// </summary>
        public class Cmovns : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovns"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovns(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if not zero (ZF == 0).
        /// </summary>
        public class Cmovnz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovnz"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovnz(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if overflow (OF == 1).
        /// </summary>
        public class Cmovo : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovo"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovo(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if parity (PF == 1).
        /// </summary>
        public class Cmovp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovp"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovp(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if parity even (PF == 1).
        /// </summary>
        public class Cmovpe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovpe"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovpe(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if parity odd (PF == 0).
        /// </summary>
        public class Cmovpo : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovpo"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovpo(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if sign (SF == 1).
        /// </summary>
        public class Cmovs : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovs"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovs(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move if zero (ZF == 1).
        /// </summary>
        public class Cmovz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmovz"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmovz(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Compare Two Operands.
        /// </summary>
        public class Cmp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first operand.
            /// </summary>
            public IOperand First => this.Operands[0];

            /// <summary>
            /// The second operand.
            /// </summary>
            public IOperand Second => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmp"/> class.
            /// </summary>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmp(IOperand first, IOperand second, string? comment = null)
            {
                this.Operands = new[] { first, second };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Compare and Exchange.
        /// </summary>
        public class Cmpxchg : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmpxchg"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmpxchg(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Compare and Exchange 8 Bytes.
        /// </summary>
        public class Cmpxchg8b : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cmpxchg8b"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cmpxchg8b(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// CPU Identification.
        /// </summary>
        public class Cpuid : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cpuid"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cpuid(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Accumulate CRC32 Value.
        /// </summary>
        public class Crc32 : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Crc32"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Crc32(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Scalar Double-Precision FP Value to Integer.
        /// </summary>
        public class Cvtsd2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cvtsd2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cvtsd2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Scalar Single-Precision FP Value to Dword Integer.
        /// </summary>
        public class Cvtss2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cvtss2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cvtss2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert with Truncation Scalar Double-Precision FP Value to Signed Integer.
        /// </summary>
        public class Cvttsd2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cvttsd2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cvttsd2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert with Truncation Scalar Single-Precision FP Value to Dword Integer.
        /// </summary>
        public class Cvttss2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Cvttss2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Cvttss2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Word to Doubleword.
        /// </summary>
        public class Cwd : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cwd"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cwd(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Word to Doubleword.
        /// </summary>
        public class Cwde : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Cwde"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Cwde(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Decimal Adjust AL after Addition.
        /// </summary>
        public class Daa : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Daa"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Daa(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Decimal Adjust AL after Subtraction.
        /// </summary>
        public class Das : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Das"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Das(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Decrement by 1.
        /// </summary>
        public class Dec : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Dec"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Dec(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Unsigned Divide.
        /// </summary>
        public class Div : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Div"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Div(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Exit MMX State.
        /// </summary>
        public class Emms : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Emms"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Emms(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Make Stack Frame for Procedure Parameters.
        /// </summary>
        public class Enter : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first operand.
            /// </summary>
            public IOperand First => this.Operands[0];

            /// <summary>
            /// The second operand.
            /// </summary>
            public IOperand Second => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Enter"/> class.
            /// </summary>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Enter(IOperand first, IOperand second, string? comment = null)
            {
                this.Operands = new[] { first, second };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Fast Exit Multimedia State.
        /// </summary>
        public class Femms : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Femms"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Femms(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Signed Divide.
        /// </summary>
        public class Idiv : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Idiv"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Idiv(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Signed Multiply.
        /// </summary>
        public class Imul : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Imul"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Imul(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Imul"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Imul(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Imul"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Imul(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Increment by 1.
        /// </summary>
        public class Inc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Inc"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Inc(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Call to Interrupt Procedure.
        /// </summary>
        public class Int : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Int"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Int(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Interrupt 4 If Overflow Flag is Set.
        /// </summary>
        public class Into : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Into"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Into(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if above (CF == 0 and ZF == 0).
        /// </summary>
        public class Ja : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Ja"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Ja(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if above or equal (CF == 0).
        /// </summary>
        public class Jae : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jae"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jae(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if below (CF == 1).
        /// </summary>
        public class Jb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jb"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jb(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if below or equal (CF == 1 or ZF == 1).
        /// </summary>
        public class Jbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jbe"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jbe(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if carry (CF == 1).
        /// </summary>
        public class Jc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jc"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jc(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if equal (ZF == 1).
        /// </summary>
        public class Je : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Je"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Je(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if ECX register is 0.
        /// </summary>
        public class Jecxz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jecxz"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jecxz(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if greater (ZF == 0 and SF == OF).
        /// </summary>
        public class Jg : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jg"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jg(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if greater or equal (SF == OF).
        /// </summary>
        public class Jge : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jge"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jge(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if less (SF != OF).
        /// </summary>
        public class Jl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jl"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jl(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if less or equal (ZF == 1 or SF != OF).
        /// </summary>
        public class Jle : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jle"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jle(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump Unconditionally.
        /// </summary>
        public class Jmp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jmp"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jmp(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not above (CF == 1 or ZF == 1).
        /// </summary>
        public class Jna : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jna"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jna(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not above or equal (CF == 1).
        /// </summary>
        public class Jnae : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnae"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnae(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not below (CF == 0).
        /// </summary>
        public class Jnb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnb"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnb(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not below or equal (CF == 0 and ZF == 0).
        /// </summary>
        public class Jnbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnbe"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnbe(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not carry (CF == 0).
        /// </summary>
        public class Jnc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnc"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnc(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not equal (ZF == 0).
        /// </summary>
        public class Jne : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jne"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jne(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not greater (ZF == 1 or SF != OF).
        /// </summary>
        public class Jng : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jng"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jng(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not greater or equal (SF != OF).
        /// </summary>
        public class Jnge : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnge"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnge(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not less (SF == OF).
        /// </summary>
        public class Jnl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnl"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnl(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not less or equal (ZF == 0 and SF == OF).
        /// </summary>
        public class Jnle : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnle"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnle(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not overflow (OF == 0).
        /// </summary>
        public class Jno : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jno"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jno(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not parity (PF == 0).
        /// </summary>
        public class Jnp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnp"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnp(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not sign (SF == 0).
        /// </summary>
        public class Jns : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jns"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jns(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if not zero (ZF == 0).
        /// </summary>
        public class Jnz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jnz"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jnz(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if overflow (OF == 1).
        /// </summary>
        public class Jo : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jo"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jo(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if parity (PF == 1).
        /// </summary>
        public class Jp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jp"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jp(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if parity even (PF == 1).
        /// </summary>
        public class Jpe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jpe"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jpe(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if parity odd (PF == 0).
        /// </summary>
        public class Jpo : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jpo"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jpo(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if sign (SF == 1).
        /// </summary>
        public class Js : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Js"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Js(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Jump if zero (ZF == 1).
        /// </summary>
        public class Jz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Jz"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Jz(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Load Status Flags into AH Register.
        /// </summary>
        public class Lahf : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Lahf"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Lahf(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Load MXCSR Register.
        /// </summary>
        public class Ldmxcsr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Ldmxcsr"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Ldmxcsr(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Load Effective Address.
        /// </summary>
        public class Lea : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Lea"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Lea(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// High Level Procedure Exit.
        /// </summary>
        public class Leave : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Leave"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Leave(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Load Fence.
        /// </summary>
        public class Lfence : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Lfence"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Lfence(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Count the Number of Leading Zero Bits.
        /// </summary>
        public class Lzcnt : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Lzcnt"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Lzcnt(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Memory Fence.
        /// </summary>
        public class Mfence : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Mfence"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Mfence(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Monitor a Linear Address Range.
        /// </summary>
        public class Monitor : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Monitor"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Monitor(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Monitor a Linear Address Range with Timeout.
        /// </summary>
        public class Monitorx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Monitorx"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Monitorx(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move.
        /// </summary>
        public class Mov : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Mov"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Mov(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move Data After Swapping Bytes.
        /// </summary>
        public class Movbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Movbe"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Movbe(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Store Doubleword Using Non-Temporal Hint.
        /// </summary>
        public class Movnti : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Movnti"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Movnti(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move with Sign-Extension.
        /// </summary>
        public class Movsx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Movsx"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Movsx(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Move with Zero-Extend.
        /// </summary>
        public class Movzx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Movzx"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Movzx(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Unsigned Multiply.
        /// </summary>
        public class Mul : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Mul"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Mul(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Unsigned Multiply Without Affecting Flags.
        /// </summary>
        public class Mulx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Mulx"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Mulx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Monitor Wait.
        /// </summary>
        public class Mwait : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Mwait"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Mwait(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Monitor Wait with Timeout.
        /// </summary>
        public class Mwaitx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Mwaitx"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Mwaitx(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Two's Complement Negation.
        /// </summary>
        public class Neg : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Neg"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Neg(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// No Operation.
        /// </summary>
        public class Nop : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Nop"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Nop(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// One's Complement Negation.
        /// </summary>
        public class Not : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Not"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Not(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Inclusive OR.
        /// </summary>
        public class Or : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Or"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Or(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Spin Loop Hint.
        /// </summary>
        public class Pause : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Pause"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Pause(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Parallel Bits Deposit.
        /// </summary>
        public class Pdep : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Pdep"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Pdep(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Parallel Bits Extract.
        /// </summary>
        public class Pext : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Pext"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Pext(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Pop a Value from the Stack.
        /// </summary>
        public class Pop : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Pop"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Pop(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Count of Number of Bits Set to 1.
        /// </summary>
        public class Popcnt : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Popcnt"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Popcnt(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Data into Caches.
        /// </summary>
        public class Prefetch : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetch"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetch(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Data Into Caches using NTA Hint.
        /// </summary>
        public class Prefetchnta : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetchnta"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetchnta(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Data Into Caches using T0 Hint.
        /// </summary>
        public class Prefetcht0 : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetcht0"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetcht0(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Data Into Caches using T1 Hint.
        /// </summary>
        public class Prefetcht1 : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetcht1"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetcht1(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Data Into Caches using T2 Hint.
        /// </summary>
        public class Prefetcht2 : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetcht2"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetcht2(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Data into Caches in Anticipation of a Write.
        /// </summary>
        public class Prefetchw : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetchw"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetchw(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Prefetch Vector Data Into Caches with Intent to Write and T1 Hint.
        /// </summary>
        public class Prefetchwt1 : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Prefetchwt1"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Prefetchwt1(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Push Value Onto the Stack.
        /// </summary>
        public class Push : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Push"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Push(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Rotate Left through Carry Flag.
        /// </summary>
        public class Rcl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Rcl"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Rcl(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Rotate Right through Carry Flag.
        /// </summary>
        public class Rcr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Rcr"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Rcr(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Read Random Number.
        /// </summary>
        public class Rdrand : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Rdrand"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Rdrand(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Read Random SEED.
        /// </summary>
        public class Rdseed : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Rdseed"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Rdseed(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Read Time-Stamp Counter.
        /// </summary>
        public class Rdtsc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Rdtsc"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Rdtsc(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Read Time-Stamp Counter and Processor ID.
        /// </summary>
        public class Rdtscp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Rdtscp"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Rdtscp(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Return from Procedure.
        /// </summary>
        public class Ret : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Ret"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Ret(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="Ret"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Ret(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Rotate Left.
        /// </summary>
        public class Rol : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Rol"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Rol(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Rotate Right.
        /// </summary>
        public class Ror : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Ror"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Ror(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Rotate Right Logical Without Affecting Flags.
        /// </summary>
        public class Rorx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Rorx"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Rorx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Store AH into Flags.
        /// </summary>
        public class Sahf : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Sahf"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Sahf(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Arithmetic Shift Left.
        /// </summary>
        public class Sal : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sal"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sal(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Arithmetic Shift Right.
        /// </summary>
        public class Sar : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sar"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sar(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Arithmetic Shift Right Without Affecting Flags.
        /// </summary>
        public class Sarx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sarx"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sarx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Subtract with Borrow.
        /// </summary>
        public class Sbb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sbb"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sbb(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if above (CF == 0 and ZF == 0).
        /// </summary>
        public class Seta : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Seta"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Seta(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if above or equal (CF == 0).
        /// </summary>
        public class Setae : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setae"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setae(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if below (CF == 1).
        /// </summary>
        public class Setb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setb"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setb(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if below or equal (CF == 1 or ZF == 1).
        /// </summary>
        public class Setbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setbe"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setbe(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if carry (CF == 1).
        /// </summary>
        public class Setc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setc"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setc(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if equal (ZF == 1).
        /// </summary>
        public class Sete : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sete"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sete(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if greater (ZF == 0 and SF == OF).
        /// </summary>
        public class Setg : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setg"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setg(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if greater or equal (SF == OF).
        /// </summary>
        public class Setge : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setge"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setge(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if less (SF != OF).
        /// </summary>
        public class Setl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setl"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setl(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if less or equal (ZF == 1 or SF != OF).
        /// </summary>
        public class Setle : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setle"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setle(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not above (CF == 1 or ZF == 1).
        /// </summary>
        public class Setna : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setna"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setna(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not above or equal (CF == 1).
        /// </summary>
        public class Setnae : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnae"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnae(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not below (CF == 0).
        /// </summary>
        public class Setnb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnb"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnb(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not below or equal (CF == 0 and ZF == 0).
        /// </summary>
        public class Setnbe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnbe"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnbe(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not carry (CF == 0).
        /// </summary>
        public class Setnc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnc"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnc(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not equal (ZF == 0).
        /// </summary>
        public class Setne : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setne"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setne(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not greater (ZF == 1 or SF != OF).
        /// </summary>
        public class Setng : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setng"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setng(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not greater or equal (SF != OF).
        /// </summary>
        public class Setnge : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnge"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnge(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not less (SF == OF).
        /// </summary>
        public class Setnl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnl"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnl(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not less or equal (ZF == 0 and SF == OF).
        /// </summary>
        public class Setnle : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnle"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnle(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not overflow (OF == 0).
        /// </summary>
        public class Setno : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setno"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setno(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not parity (PF == 0).
        /// </summary>
        public class Setnp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnp"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnp(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not sign (SF == 0).
        /// </summary>
        public class Setns : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setns"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setns(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if not zero (ZF == 0).
        /// </summary>
        public class Setnz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setnz"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setnz(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if overflow (OF == 1).
        /// </summary>
        public class Seto : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Seto"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Seto(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if parity (PF == 1).
        /// </summary>
        public class Setp : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setp"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setp(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if parity even (PF == 1).
        /// </summary>
        public class Setpe : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setpe"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setpe(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if parity odd (PF == 0).
        /// </summary>
        public class Setpo : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setpo"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setpo(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if sign (SF == 1).
        /// </summary>
        public class Sets : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sets"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sets(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set byte if zero (ZF == 1).
        /// </summary>
        public class Setz : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Setz"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Setz(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Store Fence.
        /// </summary>
        public class Sfence : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Sfence"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Sfence(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Shift Left.
        /// </summary>
        public class Shl : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Shl"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Shl(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Integer Double Precision Shift Left.
        /// </summary>
        public class Shld : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Shld"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Shld(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Shift Left Without Affecting Flags.
        /// </summary>
        public class Shlx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Shlx"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Shlx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Shift Right.
        /// </summary>
        public class Shr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Shr"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Shr(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Integer Double Precision Shift Right.
        /// </summary>
        public class Shrd : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Shrd"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Shrd(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Shift Right Without Affecting Flags.
        /// </summary>
        public class Shrx : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The 1st operand.
            /// </summary>
            public IOperand Operand1 => this.Operands[0];

            /// <summary>
            /// The 2nd operand.
            /// </summary>
            public IOperand Operand2 => this.Operands[1];

            /// <summary>
            /// The 3rd operand.
            /// </summary>
            public IOperand Operand3 => this.Operands[2];

            /// <summary>
            /// Initializes a new instance of the <see cref="Shrx"/> class.
            /// </summary>
            /// <param name="operand1">The 1st operand.</param>
            /// <param name="operand2">The 2nd operand.</param>
            /// <param name="operand3">The 3rd operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Shrx(IOperand operand1, IOperand operand2, IOperand operand3, string? comment = null)
            {
                this.Operands = new[] { operand1, operand2, operand3 };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set Carry Flag.
        /// </summary>
        public class Stc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Stc"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Stc(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Set Direction Flag.
        /// </summary>
        public class Std : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Std"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Std(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Store MXCSR Register State.
        /// </summary>
        public class Stmxcsr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Stmxcsr"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Stmxcsr(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Subtract.
        /// </summary>
        public class Sub : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Sub"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Sub(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Inverse Mask From Trailing Ones.
        /// </summary>
        public class T1mskc : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="T1mskc"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public T1mskc(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Compare.
        /// </summary>
        public class Test : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first operand.
            /// </summary>
            public IOperand First => this.Operands[0];

            /// <summary>
            /// The second operand.
            /// </summary>
            public IOperand Second => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Test"/> class.
            /// </summary>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Test(IOperand first, IOperand second, string? comment = null)
            {
                this.Operands = new[] { first, second };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Count the Number of Trailing Zero Bits.
        /// </summary>
        public class Tzcnt : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Tzcnt"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Tzcnt(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Mask From Trailing Zeros.
        /// </summary>
        public class Tzmsk : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Tzmsk"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Tzmsk(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Undefined Instruction.
        /// </summary>
        public class Ud2 : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Ud2"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Ud2(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Scalar Double-Precision FP Value to Integer.
        /// </summary>
        public class Vcvtsd2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvtsd2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvtsd2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Scalar Double-Precision Floating-Point Value to Unsigned Doubleword Integer.
        /// </summary>
        public class Vcvtsd2usi : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvtsd2usi"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvtsd2usi(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Scalar Single-Precision FP Value to Dword Integer.
        /// </summary>
        public class Vcvtss2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvtss2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvtss2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert Scalar Single-Precision Floating-Point Value to Unsigned Doubleword Integer.
        /// </summary>
        public class Vcvtss2usi : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvtss2usi"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvtss2usi(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert with Truncation Scalar Double-Precision FP Value to Signed Integer.
        /// </summary>
        public class Vcvttsd2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvttsd2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvttsd2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert with Truncation Scalar Double-Precision Floating-Point Value to Unsigned Integer.
        /// </summary>
        public class Vcvttsd2usi : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvttsd2usi"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvttsd2usi(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert with Truncation Scalar Single-Precision FP Value to Dword Integer.
        /// </summary>
        public class Vcvttss2si : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvttss2si"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvttss2si(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Convert with Truncation Scalar Single-Precision Floating-Point Value to Unsigned Integer.
        /// </summary>
        public class Vcvttss2usi : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The output operand.
            /// </summary>
            public IOperand Destination => this.Operands[0];

            /// <summary>
            /// The input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vcvttss2usi"/> class.
            /// </summary>
            /// <param name="destination">The output operand.</param>
            /// <param name="source">The input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vcvttss2usi(IOperand destination, IOperand source, string? comment = null)
            {
                this.Operands = new[] { destination, source };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Load MXCSR Register.
        /// </summary>
        public class Vldmxcsr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vldmxcsr"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vldmxcsr(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Store MXCSR Register State.
        /// </summary>
        public class Vstmxcsr : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The operand.
            /// </summary>
            public IOperand Operand => this.Operands[0];

            /// <summary>
            /// Initializes a new instance of the <see cref="Vstmxcsr"/> class.
            /// </summary>
            /// <param name="operand">The operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Vstmxcsr(IOperand operand, string? comment = null)
            {
                this.Operands = new[] { operand };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Zero All YMM Registers.
        /// </summary>
        public class Vzeroall : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Vzeroall"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Vzeroall(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Zero Upper Bits of YMM Registers.
        /// </summary>
        public class Vzeroupper : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Vzeroupper"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Vzeroupper(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Exchange and Add.
        /// </summary>
        public class Xadd : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first operand.
            /// </summary>
            public IOperand First => this.Operands[0];

            /// <summary>
            /// The second operand.
            /// </summary>
            public IOperand Second => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Xadd"/> class.
            /// </summary>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Xadd(IOperand first, IOperand second, string? comment = null)
            {
                this.Operands = new[] { first, second };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Exchange Register/Memory with Register.
        /// </summary>
        public class Xchg : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first operand.
            /// </summary>
            public IOperand First => this.Operands[0];

            /// <summary>
            /// The second operand.
            /// </summary>
            public IOperand Second => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Xchg"/> class.
            /// </summary>
            /// <param name="first">The first operand.</param>
            /// <param name="second">The second operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Xchg(IOperand first, IOperand second, string? comment = null)
            {
                this.Operands = new[] { first, second };
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Get Value of Extended Control Register.
        /// </summary>
        public class Xgetbv : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Xgetbv"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Xgetbv(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Table Look-up Translation.
        /// </summary>
        public class Xlatb : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Xlatb"/> class.
            /// </summary>
            /// <param name="comment">The optional inline comment.</param>
            public Xlatb(string? comment = null)
            {
                this.Operands = Array.Empty<IOperand>();
                this.Comment = comment;
            }
        }

        /// <summary>
        /// Logical Exclusive OR.
        /// </summary>
        public class Xor : IInstruction
        {
            /// <inheritdoc/>
            public IReadOnlyList<IOperand> Operands { get; }

            /// <inheritdoc/>
            public string? Comment { get; init; }

            /// <summary>
            /// The first input (and output) operand.
            /// </summary>
            public IOperand Target => this.Operands[0];

            /// <summary>
            /// The second input operand.
            /// </summary>
            public IOperand Source => this.Operands[1];

            /// <summary>
            /// Initializes a new instance of the <see cref="Xor"/> class.
            /// </summary>
            /// <param name="target">The first input (and output) operand.</param>
            /// <param name="source">The second input operand.</param>
            /// <param name="comment">The optional inline comment.</param>
            public Xor(IOperand target, IOperand source, string? comment = null)
            {
                this.Operands = new[] { target, source };
                this.Comment = comment;
            }
        }

        #endregion Generated
    }
}
