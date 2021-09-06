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

        public bool AllowMultiple => true;

        public AttributeTargets Targets => AttributeTargets.Instruction | AttributeTargets.Assembly | AttributeTargets.BasicBlock
            | AttributeTargets.Procedure;

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
            var ctx = new Context();
            ctx.WithInstructionSyntax("nop", _ => new Instruction.Nop(), (_, _) => { })
               .WithInstructionSyntax("ret", _ => new Instruction.Ret(), (_, _) => { });
            ctx.WithAttributeDefinition(new FooDefinition());

            var src = @"
[assembly: foo]
[assembly: foo]

procedure hello() [foo]:
block owo [block: foo]:
  nop [foo, foo, foo]
  ret
";
            var lexer = new IrLexer(src);
            var parser = new IrParser(ctx, lexer);
            var asm = parser.ParseAssembly();

            var writer = new IrWriter(ctx, Console.Out);
            writer.WriteAssembly(asm);
        }
    }
}
