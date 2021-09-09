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
            ctx.WithTypeDefinition("i32", new Type.Int(32));
            ctx
                .WithInstructionSyntax("nop", _ => new Instruction.Nop(), (_, _) => { })
                .WithInstructionSyntax("ret",
                _ => new Instruction.Ret(),
                (ins, writer) =>
                {
                    if (ins.Value is not null)
                    {
                        writer.Underlying.Write(' ');
                        // TODO: Write value
                        throw new NotImplementedException();
                    }
                })
                .WithInstructionSyntax("add",
                parser =>
                {
                    var type = parser.ParseType();
                    var left = parser.ParseValue(type);
                    parser.Expect(IrTokenType.Comma);
                    var right = parser.ParseValue(type);
                    return new Instruction.Add(left, right);
                },
                (ins, writer) =>
                {
                    writer.Underlying.Write(' ');
                    writer.WriteType(ins.Left.Type);
                    writer.Underlying.Write(' ');
                    writer.WriteValue(ins.Left);
                    writer.Underlying.Write(", ");
                    writer.WriteValue(ins.Right);
                });
            ctx.WithAttributeDefinition(new FooDefinition());

            var src = @"
[assembly: foo]
[assembly: foo]

procedure hello(): [foo]
block owo: [block: foo]
  v1 = add i32 1, 2
  v2 = add i32 v2, 3
  nop [foo, foo, foo]
  ret

procedure aaa() [foo]
";
            var lexer = new IrLexer(src);
            var parser = new IrParser(ctx, lexer);
            var asm = parser.ParseAssembly();

            var writer = new IrWriter(ctx, Console.Out);
            writer.WriteAssembly(asm);
        }
    }
}
