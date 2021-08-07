using System;
using Yoakke.Collections;
using Yoakke.Ir.Model;
using Yoakke.Ir.Passes;
using Yoakke.Ir.Runtime;
using Yoakke.Ir.Writers;
using Type = Yoakke.Ir.Model.Type;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var i32 = new Type.Int(true, 32);

            Value MakeInt(int v) => new Value.Int(true, new BigInt(32, BitConverter.GetBytes(v)));

            var builder = new AssemblyBuilder()
                .DefineProcedure("foo", out var foo)
                .DefineLocal(i32)
                .DefineParameter(i32, out var arg0)
                .DefineParameter(i32, out var arg1)
                .IntAdd(arg0, arg1, out var added)
                .Ret(added);
            foo.Return = i32;

            builder.DefineProcedure("main", out var main);
            var lastBb = builder.CurrentBasicBlock;
            builder.DefineBasicBlock(out var thenBb);
            builder.DefineBasicBlock(out var elseBb);
            builder.CurrentBasicBlock = lastBb;
            builder.JumpIf(MakeInt(0), new Value.BasicBlock(thenBb), new Value.BasicBlock(elseBb));
            builder.CurrentBasicBlock = thenBb;
            builder
                .Call(new Value.Proc(foo), new Value[] { MakeInt(1), MakeInt(2) }.AsValue(), out var callRes)
                .Ret(callRes);
            builder.CurrentBasicBlock = elseBb;
            builder
                .Call(new Value.Proc(foo), new Value[] { MakeInt(4), MakeInt(7) }.AsValue(), out var callRes2)
                .Ret(callRes2);
            main.Return = i32;

            var pass = new RemoveUnreferencedLocals();
            foreach (var proc in builder.Assembly.Procedures.Values) pass.Pass(proc);
        
            var writer = new AssemblyTextWriter();
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);

            var vm = new ModelVirtualMachine(builder.Assembly);
            var result = vm.Execute(new Value.Proc(main), new Value[] { });
            Console.WriteLine($"\n\nProcess returned {((Value.Int)result).Value.ToString(true)}");
        }
    }
}
