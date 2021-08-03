// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Ir.Model;
using Type = Yoakke.Ir.Model.Type;

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
        /// <param name="result">The defined <see cref="IProcedure"/> gets
        /// written here.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineProcedure(string name, out IProcedure result)
        {
            var proc = new Procedure(name);
            var bb = new BasicBlock(proc);
            proc.BasicBlocks.Add(bb);
            this.Assembly.Procedures.Add(name, proc);
            this.currentProcedure = proc;
            this.currentBasicBlock = bb;
            result = proc;
            return this;
        }

        /// <summary>
        /// Defines a new <see cref="IProcedure"/> in the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="name">The logical name of the procedure.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineProcedure(string name) => this.DefineProcedure(name, out _);

        /// <summary>
        /// Defines a new parameter in the current <see cref="IProcedure"/>.
        /// </summary>
        /// <param name="type">The type of the argument to define.</param>
        /// <param name="result">The <see cref="Value"/> gets written here
        /// that can be used to reference the defined parameter.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineParameter(Type type, out Value result)
        {
            var proc = this.CurrentProcedure;
            var param = new Parameter(type);
            result = new Value.Argument(param);
            proc.Parameters.Add(param);
            return this;
        }

        /// <summary>
        /// Defines a new parameter in the current <see cref="IProcedure"/>.
        /// </summary>
        /// <param name="type">The type of the argument to define.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineParameter(Type type) => this.DefineParameter(type, out _);

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
        public AssemblyBuilder DefineBasicBlock() => this.DefineBasicBlock(out _);

        /// <summary>
        /// Defines a local in the current <see cref="IProcedure"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the local to define.</param>
        /// <param name="result">The reference to the local gets written here.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineLocal(Type type, out Value result)
        {
            var proc = this.CurrentProcedure;
            var local = new Local(type);
            result = new Value.Local(local);
            proc.Locals.Add(local);
            return this;
        }

        /// <summary>
        /// Defines a <see cref="Local"/> in the current <see cref="IProcedure"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the <see cref="Local"/> to define.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder DefineLocal(Type type) => this.DefineLocal(type, out _);

        /// <summary>
        /// Writes an <see cref="Instruction"/> to the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Write(Instruction instruction)
        {
            this.CurrentBasicBlock.Instructions.Add(instruction);
            return this;
        }

        #region Instructions

        /// <summary>
        /// Writes a return instruction.
        /// </summary>
        /// <param name="value">The optional <see cref="Value"/> to return.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder Ret(Value? value = null) => this.Write(new Instruction.Ret(value));

        /// <summary>
        /// Writes an integer addition instruction.
        /// </summary>
        /// <param name="left">The first operand to add.</param>
        /// <param name="right">The second operand to add.</param>
        /// <param name="result">The <see cref="Value"/> gets written here
        /// that can be used to reference the result.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder IntAdd(Value left, Value right, out Value result) =>
            this.WriteValueProducer(new Instruction.IntAdd(left, right), out result);

        /// <summary>
        /// Writes an integer addition instruction.
        /// </summary>
        /// <param name="left">The first operand to add.</param>
        /// <param name="right">The second operand to add.</param>
        /// <returns>This instance to chain calls.</returns>
        public AssemblyBuilder IntAdd(Value left, Value right) => this.IntAdd(left, right, out _);

        #endregion

        private AssemblyBuilder WriteValueProducer(Instruction.ValueProducer ins, out Value result)
        {
            var bb = this.CurrentBasicBlock;
            result = new Value.Temp(ins);
            bb.Instructions.Add(ins);
            return this;
        }
    }
}
