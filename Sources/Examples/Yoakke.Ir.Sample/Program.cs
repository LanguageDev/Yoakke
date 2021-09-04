using System;
using System.Collections.Generic;
using Yoakke.Ir.Model;
using Yoakke.Ir.Model.Attributes;
using Yoakke.Ir.Model.Builders;
using Yoakke.Ir.Syntax;
using AttributeTargets = Yoakke.Ir.Model.Attributes.AttributeTargets;
using Type = Yoakke.Ir.Model.Type;

namespace Yoakke.Ir.Sample
{
    public class FooDefinition : IAttributeDefinition
    {
        public string Name => "foo";

        public bool AllowMultiple => false;

        public AttributeTargets Targets => AttributeTargets.Instruction;

        public IReadOnlyList<Type> ParameterTypes => Array.Empty<Type>();

        public IAttribute Instantiate(IReadOnlyList<Constant> arguments) => new Foo(this);
    }

    public class Foo : IAttribute
    {
        public IAttributeDefinition Definition { get; }

        public IReadOnlyList<Constant> Arguments => Array.Empty<Constant>();

        public Foo(FooDefinition definition) => this.Definition = definition;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var asm = new AssemblyBuilder();
            var proc = new ProcedureBuilder("main");
            var bb = new BasicBlockBuilder("entry")
                .Nop()
                .Ret();

            asm.WithProcedure(proc);
            proc.WithEntryAt(bb);

            var fooAttr = new FooDefinition();
            var foo = fooAttr.Instantiate(Array.Empty<Constant>());
            bb.Instructions[0].AddAttribute(foo);

            var writer = new IrWriter(Console.Out);
            writer.WriteAssembly(asm);
        }
    }
}
