namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
#if false
            var i32 = new Type.Int(true, 32);

            Value MakeInt(int v) => new Value.Int(true, new BigInt(32, BitConverter.GetBytes(v)));

            var builder = new AssemblyBuilder()
                .DefineProcedure("fib", out var fib)
                .DefineParameter(i32, out var n);
            var lastBb = builder.CurrentBasicBlock;
            builder.DefineBasicBlock(out var thenBb);
            builder.DefineBasicBlock(out var elseBb);
            builder.CurrentBasicBlock = lastBb;
            builder
                .Cmp(Comparison.LessEqual, n, MakeInt(1), out var cmpResult)
                .JumpIf(cmpResult, new Value.BasicBlock(thenBb), new Value.BasicBlock(elseBb));
            builder.CurrentBasicBlock = thenBb;
            builder.Ret(MakeInt(1));
            builder.CurrentBasicBlock = elseBb;
            builder
                .Sub(n, MakeInt(1), out var nMinusOne)
                .Sub(n, MakeInt(2), out var nMinusTwo)
                .Call(new Value.Proc(fib), new Value[] { nMinusOne }.AsValue(), out var fibNminusOne)
                .Call(new Value.Proc(fib), new Value[] { nMinusTwo }.AsValue(), out var fibNminusTwo)
                .Add(fibNminusOne, fibNminusTwo, out var fibRes)
                .Ret(fibRes);
            fib.Return = i32;

            builder
                .DefineProcedure("main", out var main)
                .Call(new Value.Proc(fib), new Value[] { MakeInt(10) }.AsValue(), out var callRes)
                .Ret(callRes);

            var pass = new RemoveUnreferencedLocals();
            foreach (var proc in builder.Assembly.Procedures.Values) pass.Pass(proc);
        
            var writer = new AssemblyTextWriter();
            writer.Write(builder.Assembly);
            Console.WriteLine(writer.Result);

            var vm = new ModelVirtualMachine(builder.Assembly);
            var result = vm.Execute(new Value.Proc(main), new Value[] { });
            Console.WriteLine($"\n\nProcess returned {((Value.Int)result).Value.ToString(true)}");
#endif
        }
    }
}
