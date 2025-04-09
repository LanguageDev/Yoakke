// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using BenchmarkDotNet.Running;

if (args.Length == 1 && args[0] == "parser")
{
    new Yoakke.SynKit.Parser.Benchmarks.ExpressionBenchmarks().ExpressionParser();
}
else if (args.Length == 1 && args[0] == "manual_parser")
{
    new Yoakke.SynKit.Parser.Benchmarks.ExpressionBenchmarks().ManualExpressionParser();
}
else if (args.Length == 1 && args[0] == "worst_parser")
{
    new Yoakke.SynKit.Parser.Benchmarks.ExpressionBenchmarks().WorstManualExpressionParser();
}
else if (args.Length == 1 && args[0] == "complex")
{
    new Yoakke.SynKit.Parser.Benchmarks.ExpressionBenchmarks().ComplexExpressionParser();
}
else if (args.Length == 1 && args[0] == "manual_complex")
{
    new Yoakke.SynKit.Parser.Benchmarks.ExpressionBenchmarks().ManualComplexExpressionParser();
}
else
{
   BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
}
