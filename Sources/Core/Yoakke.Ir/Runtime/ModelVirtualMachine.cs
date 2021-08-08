// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;
using Yoakke.Ir.Model;
using Type = Yoakke.Ir.Model.Type;

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
        private readonly Dictionary<Value, int> valuesToAddress = new();
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
                this.valuesToAddress.Add(new Value.Proc(proc), this.instructions.Count);
                foreach (var bb in proc.BasicBlocks)
                {
                    // TODO
                    this.valuesToAddress.Add(new Value.BasicBlock(bb), this.instructions.Count);
                    this.instructions.AddRange(bb.Instructions);
                }
            }
        }

        /// <summary>
        /// Executes a given procedure.
        /// </summary>
        /// <param name="proc">The procedure to execute.</param>
        /// <param name="args">The arguments to call the procedure with.</param>
        /// <returns>The value that the called procedure returns.</returns>
        public Value Execute(Value proc, IReadOnlyList<Value> args)
        {
            var returnValue = Value.Void.Instance;
            var stackSize = this.callStack.Count;
            this.Call(proc, args, v => returnValue = v);
            for (; this.callStack.Count != stackSize; this.ExecuteCycle())
            {
                // Pass
            }
            return returnValue;
        }

        /// <summary>
        /// Executes the next instruction.
        /// </summary>
        public void ExecuteCycle()
        {
            var instruction = this.instructions[this.InstructionPointer++];
            switch (instruction)
            {
            case Instruction.Call call:
            {
                // Construct the action that describes returning
                var locals = this.callStack.Peek().Locals;
                var result = new Value.Temp(call);
                var proc = this.Unwrap(call.Procedure);
                // Do the call
                this.Call(call.Procedure, call.Arguments, value => locals[result] = value);
            }
            break;

            case Instruction.Ret ret:
            {
                // Evaluate return value
                var returnValue = Value.Void.Instance;
                if (ret.Value is not null) returnValue = this.Unwrap(ret.Value);
                // Pop stack
                var top = this.callStack.Pop();
                // Write return value
                top.WriteReturnValue(returnValue);
                // Jump
                this.InstructionPointer = top.ReturnAddress;
            }
            break;

            case Instruction.Jump jump:
            {
                var target = (Value.BasicBlock)this.Unwrap(jump.Target);
                this.InstructionPointer = this.valuesToAddress[target];
            }
            break;

            case Instruction.JumpIf jumpIf:
            {
                var condition = (Value.Int)this.Unwrap(jumpIf.Condition);
                var then = (Value.BasicBlock)this.Unwrap(jumpIf.Then);
                var @else = (Value.BasicBlock)this.Unwrap(jumpIf.Else);
                this.InstructionPointer = this.valuesToAddress[condition.Value.IsZero ? @else : then];
            }
            break;

            case Instruction.ValueProducer valProducer:
            {
                var value = this.Evaluate(valProducer);
                var top = this.callStack.Peek();
                var temp = new Value.Temp(valProducer);
                top.Locals[temp] = value;
            }
            break;

            default: throw new NotImplementedException();
            }
        }

        private Value Evaluate(Instruction.ValueProducer valueProducer)
        {
            switch (valueProducer)
            {
            case Instruction.Binary bin:
                var left = this.Unwrap(bin.Left);
                var right = this.Unwrap(bin.Right);
                var resultType = bin.ResultType;
                return EvaluateBinary(bin, resultType, left, right);

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

        private void Call(Value procValue, IReadOnlyList<Value> args, Action<Value> writeReturnValue)
        {
            var proc = ((Value.Proc)procValue).Procedure;
            // Create the new frame and tell it where to return the value
            var top = new StackFrame(this.InstructionPointer, writeReturnValue);
            // Register arguments
            for (var i = 0; i < proc.Parameters.Count; ++i)
            {
                var param = new Value.Argument(proc.Parameters[i]);
                top.Locals[param] = this.Unwrap(args[i]);
            }
            // Push, jump
            this.callStack.Push(top);
            this.InstructionPointer = this.valuesToAddress[procValue];
        }

        private static Value EvaluateBinary(Instruction.Binary bin, Type resultType, Value left, Value right) => resultType switch
        {
            Type.Int => bin switch
            {
                Instruction.Add => EvaluateBinaryInt(resultType, left, right, (a, b) => a + b),
                Instruction.Sub => EvaluateBinaryInt(resultType, left, right, (a, b) => a - b),
                Instruction.Cmp cmp => EvaluateIntComparison(cmp.Comparison, left, right),

                _ => throw new NotImplementedException(),
            },

            _ => throw new NotImplementedException(),
        };

        private static Value EvaluateBinaryInt(Type resultType, Value left, Value right, Func<BigInt, BigInt, BigInt> eval) =>
            new Value.Int(((Type.Int)resultType).Signed, eval(((Value.Int)left).Value, ((Value.Int)right).Value));

        private static Value EvaluateIntComparison(Comparison comparison, Value left, Value right)
        {
            var leftType = (Type.Int)left.Type;
            var leftValue = ((Value.Int)left).Value;
            var rightValue = ((Value.Int)right).Value;
            var result = leftType.Signed
                ? leftValue.SignedCompareTo(rightValue)
                : leftValue.UnsignedCompareTo(rightValue);
            return MakeBool(comparison switch
            {
                Comparison.Less => result < 0,
                Comparison.LessEqual => result <= 0,
                Comparison.Greater => result > 0,
                Comparison.GreaterEqual => result >= 0,
                Comparison.Equal => result == 0,
                Comparison.NotEqual => result != 0,
                _ => throw new InvalidOperationException(),
            });
        }

        private static Value MakeBool(bool value) =>
            new Value.Int(false, new BigInt(1, new[] { (byte)(value ? 1 : 0) }));
    }
}
