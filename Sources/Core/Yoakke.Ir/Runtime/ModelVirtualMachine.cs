// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Ir.Model;

namespace Yoakke.Ir.Runtime
{
    /// <summary>
    /// A virtual machine that interprets the model IR code as-is.
    /// </summary>
    public class ModelVirtualMachine
    {
        private class StackFrame
        {
            public int ReturnAddress { get; }

            public Action<Value> WriteReturnValue { get; }

            public Dictionary<Value, Value> Locals { get; } = new();

            public StackFrame(int returnAddress, Action<Value> writeReturnValue)
            {
                this.ReturnAddress = returnAddress;
                this.WriteReturnValue = writeReturnValue;
            }
        }

        private readonly IReadOnlyAssembly assembly;
        private readonly List<Instruction> instructions = new();
        private readonly Dictionary<IReadOnlyProcedure, int> procToAddress = new();
        private readonly Dictionary<IReadOnlyBasicBlock, int> bbToAddress = new();
        private readonly Stack<StackFrame> callStack = new();

        /// <summary>
        /// The current instruction pointer.
        /// </summary>
        public int InstructionPointer { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelVirtualMachine"/> class.
        /// </summary>
        /// <param name="assembly">The <see cref="IReadOnlyAssembly"/> to execute.</param>
        public ModelVirtualMachine(IReadOnlyAssembly assembly)
        {
            this.assembly = assembly;

            // Flatten our assembly
            foreach (var proc in assembly.Procedures.Values)
            {
                this.procToAddress.Add(proc, this.instructions.Count);
                foreach (var bb in proc.BasicBlocks)
                {
                    this.bbToAddress.Add(bb, this.instructions.Count);
                    this.instructions.AddRange(bb.Instructions);
                }
            }
        }

        /// <summary>
        /// Executes the next instruction.
        /// </summary>
        public void ExecuteCycle()
        {
            var instruction = this.instructions[this.InstructionPointer++];
            switch (instruction)
            {
            case Instruction.Ret ret:
                var returnValue = ret.Value is null ? Value.Void.Instance : this.Unwrap(ret.Value);
                var top = this.callStack.Pop();
                top.WriteReturnValue(returnValue);
                break;

            case Instruction.IntAdd intAdd:
                var left = this.Unwrap(intAdd.Left);
                var right = this.Unwrap(intAdd.Right);
                // TODO
                throw new NotImplementedException();
                break;

            default: throw new NotImplementedException();
            }
        }

        private Value Unwrap(Value value) => value switch
        {
               Value.Argument
            or Value.Local
            or Value.Temp => this.callStack.Peek().Locals[value],
            _ => value,
        };
    }
}
