// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Ir
{
    /// <summary>
    /// A builder to help constructing <see cref="IAssembly"/>s.
    /// </summary>
    public class AssemblyBuilder
    {
        private IProcedure? currentProcedure;
        private IBasicBlock? currentBasicBlock;

        /// <summary>
        /// The built <see cref="IAssembly"/>.
        /// </summary>
        public IAssembly Assembly { get; } = new Assembly();

        /// <summary>
        /// The current <see cref="IProcedure"/> we are building.
        /// </summary>
        public IProcedure CurrentProcedure
        {
            get => this.currentProcedure ?? throw new InvalidOperationException("No procedure is being built");
            set
            {
                this.currentProcedure = value;
                this.currentBasicBlock = this.currentProcedure.BasicBlocks.Count > 0
                    ? this.currentProcedure.BasicBlocks[^1]
                    : null;
            }
        }

        /// <summary>
        /// The current <see cref="IBasicBlock"/> we are building.
        /// </summary>
        public IBasicBlock CurrentBasicBlock
        {
            get => this.currentBasicBlock ?? throw new InvalidOperationException("No basic block is being built");
            set
            {
                this.currentBasicBlock = value;
                this.currentProcedure = this.currentBasicBlock.Procedure;
            }
        }

        /// <summary>
        /// Defines a new <see cref="IProcedure"/> in the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="name">The logical name of the procedure.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineProcedure(string name)
        {
            var proc = new Procedure(name);
            var bb = new BasicBlock(proc);
            proc.BasicBlocks.Add(bb);
            this.Assembly.Procedures.Add(name, proc);
            this.currentProcedure = proc;
            this.currentBasicBlock = bb;
            return this;
        }

        /// <summary>
        /// Defines a new <see cref="IBasicBlock"/> in the current <see cref="IProcedure"/>.
        /// </summary>
        /// <param name="result">The new <see cref="IBasicBlock"/> gets written here.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineBasicBlock(out IBasicBlock result)
        {
            var proc = this.CurrentProcedure;
            result = new BasicBlock(proc);
            proc.BasicBlocks.Add(result);
            this.currentBasicBlock = result;
            return this;
        }

        /// <summary>
        /// Defines a new <see cref="IBasicBlock"/> in the current <see cref="IProcedure"/>.
        /// </summary>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineBasicBlock() => this.DefineBasicBlock(out var _);

        /// <summary>
        /// Writes an <see cref="IInstruction"/> to the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="IInstruction"/> to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Write(IInstruction instruction)
        {
            this.CurrentBasicBlock.Instructions.Add(instruction);
            return this;
        }

        #region Instructions

        /// <summary>
        /// Writes a local allocation instruction.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the local to allocate.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Local(Type type) => this.Write(new Instruction.Local(type));

        /// <summary>
        /// Writes a return instruction.
        /// </summary>
        /// <param name="value">The optional <see cref="Value"/> to return.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ret(Value? value = null) => this.Write(new Instruction.Ret(value));

        #endregion
    }
}
