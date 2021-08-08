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

namespace Yoakke.Ir.Writers
{
    /// <summary>
    /// Basic class for writing IR assembly.
    /// </summary>
    public class AssemblyTextWriter
    {
        // Just for friendly names we keep a context per-procedure
        private class ProcedureContext
        {
            public IReadOnlyDictionary<Parameter, int> Parameters { get; init; } = new Dictionary<Parameter, int>();

            public IReadOnlyDictionary<Local, int> Locals { get; init; } = new Dictionary<Local, int>();

            public IReadOnlyDictionary<IReadOnlyBasicBlock, int> Blocks { get; init; } = new Dictionary<IReadOnlyBasicBlock, int>();

            public IReadOnlyDictionary<Instruction, int> Temporaries { get; init; } = new Dictionary<Instruction, int>();
        }

        /// <summary>
        /// The underlying <see cref="StringBuilder"/> this <see cref="AssemblyTextWriter"/> writes to.
        /// </summary>
        public StringBuilder Result { get; }

        private ProcedureContext procedureContext = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTextWriter"/> class.
        /// </summary>
        /// <param name="result">The <see cref="StringBuilder"/> to write the code to.</param>
        public AssemblyTextWriter(StringBuilder result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyTextWriter"/> class.
        /// </summary>
        public AssemblyTextWriter()
            : this(new StringBuilder())
        {
        }

        /// <summary>
        /// Writes a character to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="c">The character to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(char c)
        {
            this.Result.Append(c);
            return this;
        }

        /// <summary>
        /// Writes a string to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(string str)
        {
            this.Result.Append(str);
            return this;
        }

        /// <summary>
        /// Starts a new line for the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine()
        {
            this.Result.AppendLine();
            return this;
        }

        /// <summary>
        /// Writes a string to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="str">The string to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(string str) => this.Write(str).WriteLine();

        /// <summary>
        /// Writes an object to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(object? obj) => this.Write(obj?.ToString() ?? "null");

        /// <summary>
        /// Writes an object to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="obj">The object to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(object? obj) => this.Write(obj).WriteLine();

        /// <summary>
        /// Writes an <see cref="IReadOnlyAssembly"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="IReadOnlyAssembly"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(IReadOnlyAssembly assembly)
        {
            var first = true;
            foreach (var proc in assembly.Procedures.Values)
            {
                if (!first) this.WriteLine().WriteLine();
                first = false;
                this.Write(proc);
            }
            return this;
        }

        /// <summary>
        /// Writes an <see cref="IReadOnlyAssembly"/> to the underlying <see cref="StringBuilder"/>
        /// and goes to the next line.
        /// </summary>
        /// <param name="assembly">The <see cref="IReadOnlyAssembly"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(IReadOnlyAssembly assembly) => this.Write(assembly).WriteLine();

        /// <summary>
        /// Writes an <see cref="IReadOnlyProcedure"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="procedure">The <see cref="IReadOnlyProcedure"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(IReadOnlyProcedure procedure)
        {
            // First set the context
            this.ConstructProcedureContext(procedure);
            // Write the procedure header, which is
            // proc name(arg(0) type0, arg(1) type1, ...) return_type:
            this.Write("proc ").Write(procedure.Name).Write("(");
            for (var i = 0; i < procedure.Parameters.Count; ++i)
            {
                if (i > 0) this.Write(", ");
                this.Write("arg(").Write(i).Write(") ").Write(procedure.Parameters[i].Type);
            }
            this.Write(") ").Write(procedure.Return).Write(':');

            // Locals before the first basic block in the form
            // local(i) type_i
            for (var i = 0; i < procedure.Locals.Count; ++i)
            {
                this.WriteLine().Write("  local(").Write(i).Write(", ").Write(procedure.Locals[i].Type).Write(')');
            }

            foreach (var bb in procedure.BasicBlocks) this.WriteLine().Write(bb);
            return this;
        }

        /// <summary>
        /// Writes an <see cref="IReadOnlyProcedure"/> to the underlying <see cref="StringBuilder"/>
        /// and goes to the next line.
        /// </summary>
        /// <param name="procedure">The <see cref="IReadOnlyProcedure"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(IReadOnlyProcedure procedure) => this.Write(procedure).WriteLine();

        /// <summary>
        /// Writes an <see cref="IReadOnlyBasicBlock"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="basicBlock">The <see cref="IReadOnlyBasicBlock"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(IReadOnlyBasicBlock basicBlock)
        {
            this.Write("block(").Write(this.procedureContext.Blocks[basicBlock]).Write("):");
            foreach (var ins in basicBlock.Instructions)
            {
                this.WriteLine().Write("  ");
                if (ins is Instruction.ValueProducer v)
                {
                    this.Write("temp(").Write(this.procedureContext.Temporaries[v]).Write(") ").Write(v.ResultType).Write(" = ");
                }
                this.Write(ins);
            }
            return this;
        }

        /// <summary>
        /// Writes an <see cref="IReadOnlyBasicBlock"/> to the underlying <see cref="StringBuilder"/>
        /// and goes to the next line.
        /// </summary>
        /// <param name="basicBlock">The <see cref="IReadOnlyBasicBlock"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(IReadOnlyBasicBlock basicBlock) => this.Write(basicBlock).WriteLine();

        /// <summary>
        /// Writes an <see cref="Instruction"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(Instruction instruction)
        {
            switch (instruction)
            {
            case Instruction.Call call:
                this.Write("call ").Write(call.Procedure).Write('(');
                var first = true;
                foreach (var arg in call.Arguments)
                {
                    if (!first) this.Write(", ");
                    first = false;
                    this.Write(arg);
                }
                return this.Write(')');

            case Instruction.Ret ret:
                return ret.Value is null
                    ? this.Write("ret")
                    : this.Write("ret ").Write(ret.Value);

            case Instruction.Jump jump:
                return this.Write("jump ").Write(jump.Target);

            case Instruction.JumpIf jump:
                return this.Write("jump_if ").Write(jump.Condition)
                    .Write(" then ").Write(jump.Then)
                    .Write(" else ").Write(jump.Else);

            case Instruction.Add add:
                return this.Write("add ").Write(add.Left).Write(", ").Write(add.Right);

            case Instruction.Sub sub:
                return this.Write("sub ").Write(sub.Left).Write(", ").Write(sub.Right);

            case Instruction.Cmp cmp:
                return this.Write("cmp ").Write(ComparisonToString(cmp.Comparison))
                    .Write(' ').Write(cmp.Left)
                    .Write(", ").Write(cmp.Right);

            default: throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Writes an <see cref="Instruction"/> to the underlying <see cref="StringBuilder"/>
        /// and goes to the next line.
        /// </summary>
        /// <param name="instruction">The <see cref="Instruction"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(Instruction instruction) => this.Write(instruction).WriteLine();

        /// <summary>
        /// Writes a <see cref="Value"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(Value value) => value switch
        {
            Value.Proc p => this.Write(p.Procedure.Name),
            Value.BasicBlock b => this.Write("block(").Write(this.procedureContext.Blocks[b.Block]).Write(")"),
            Value.Argument a => this.Write("arg(").Write(this.procedureContext.Parameters[a.Parameter]).Write(')'),
            Value.Local l => this.Write("local(").Write(this.procedureContext.Locals[l.Definition]).Write(')'),
            Value.Temp t => this.Write("temp(").Write(this.procedureContext.Temporaries[t.Instruction]).Write(')'),
            Value.Int i => this.Write(i.Type).Write('(').Write(i.Value.ToString(((Type.Int)i.Type).Signed)).Write(')'),

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Writes a <see cref="Value"/> to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="value">The <see cref="Value"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(Value value) => this.Write(value).WriteLine();

        /// <summary>
        /// Writes a <see cref="Type"/> to the underlying <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter Write(Type type) => type switch
        {
            Type.Void => this.Write("void"),
            Type.Int i => this.Write(i.Signed ? 'i' : 'u').Write(i.Bits),

            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Writes a <see cref="Type"/> to the underlying <see cref="StringBuilder"/> and goes to the next line.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to write.</param>
        /// <returns>This instance to be able to chain calls.</returns>
        public AssemblyTextWriter WriteLine(Type type) => this.Write(type).WriteLine();

        private void ConstructProcedureContext(IReadOnlyProcedure procedure)
        {
            var valueIns = procedure.BasicBlocks
                .SelectMany(bb => bb.Instructions)
                .OfType<Instruction.ValueProducer>()
                .Cast<Instruction>();

            this.procedureContext = new()
            {
                Parameters = ToIndexDictionary(procedure.Parameters),
                Locals = ToIndexDictionary(procedure.Locals),
                Blocks = ToIndexDictionary(procedure.BasicBlocks),
                Temporaries = ToIndexDictionary(valueIns),
            };
        }

        private static IReadOnlyDictionary<T, int> ToIndexDictionary<T>(IEnumerable<T> values)
            where T : notnull => values
            .Select((v, idx) => new KeyValuePair<T, int>(v, idx))
            .ToDictionary(kv => kv.Key, kv => kv.Value);

        private static string ComparisonToString(Comparison comparison) => comparison switch
        {
            Comparison.Less => "le",
            Comparison.LessEqual => "le_eq",
            Comparison.Greater => "gr",
            Comparison.GreaterEqual => "gr_eq",
            Comparison.Equal => "eq",
            Comparison.NotEqual => "ne",

            _ => throw new InvalidOperationException(),
        };
    }
}
